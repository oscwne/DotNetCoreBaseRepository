using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;
using Salesgram.Core.Enum;
using Salesgram.Core.Extension;
using Salesgram.Core.Feature;
using Salesgram.Core.Wrapper;

namespace Salesgram.Core.Middleware
{
    public class ResponseMiddleware
    {
        private readonly RequestDelegate _next;

        public ResponseMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (IsSwagger(context))
                await this._next(context);
            else
            {
                var originalBodyStream = context.Response.Body;

                using (var responseBody = new MemoryStream())
                {
                    context.Response.Body = responseBody;

                    try
                    {
                        await _next.Invoke(context);
                        var statusCode = context.Response.StatusCode;
                        if (statusCode == (int)HttpStatusCode.OK || statusCode == (int)HttpStatusCode.Created)
                        {
                            var body = await FormatResponse(context.Response);
                            await HandleSuccessRequestAsync(context, body, context.Response.StatusCode);
                        }
                        else
                        {
                            await HandleNotSuccessRequestAsync(context, context.Response.StatusCode);
                        }
                    }
                    catch (Exception ex)
                    {
                        await HandleExceptionAsync(context, ex);
                    }
                    finally
                    {
                        responseBody.Seek(0, SeekOrigin.Begin);
                        await responseBody.CopyToAsync(originalBodyStream);
                    }
                }
            }

        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var request = context.Request;
            var response = new ResponseWrapper<ExceptionWrapper>();
            response.Message = ResponseMessageTypeEnum.Exception.GetDescription();

#if DEBUG
            response.Result = new ExceptionWrapper
            {
                DateTime = DateTime.UtcNow,
                RequestUri = new Uri(request.GetDisplayUrl()),
                Exception = exception
            };
#else
            response.Result = new ExceptionWrapper
            {
                DateTime = DateTime.UtcNow,
                RequestUri = new Uri(request.GetDisplayUrl())
            };
#endif



            var json = JsonConvert.SerializeObject(response);

            return context.Response.WriteAsync(json);
        }

        private static Task HandleNotSuccessRequestAsync(HttpContext context, int code)
        {
            context.Response.Clear();
            context.Response.ContentType = "application/json";

            var modelState = context.Features.Get<ModelStateFeature>()?.ModelState;
            var response = new ResponseWrapper<string>();

            if (modelState != null && modelState.Any(m => m.Value.Errors.Count > 0))
            {
                var errors = modelState.Keys
                .SelectMany(key => modelState[key].Errors.Select(x => new ValidationErrorWrapper(key, x.ErrorMessage)))
                .ToList();

                response.Errors = errors;
                response.Message = response.Message = ResponseMessageTypeEnum.ValidationError.GetDescription();
            }
            else
            {
                response.Message = ResponseMessageTypeEnum.Failure.GetDescription();
            }

            context.Response.StatusCode = code;

            var json = JsonConvert.SerializeObject(response);

            return context.Response.WriteAsync(json);
        }

        private static Task HandleSuccessRequestAsync(HttpContext context, object body, int code)
        {
            context.Response.ContentType = "application/json";
            string jsonString;
            string bodyText = !body.ToString().IsValidJson() ? JsonConvert.SerializeObject(body) : body.ToString();

            dynamic bodyContent = JsonConvert.DeserializeObject<dynamic>(bodyText);
            bodyContent.message = bodyContent.message ?? ResponseMessageTypeEnum.Success.GetDescription();

            jsonString = JsonConvert.SerializeObject(bodyContent);

            return context.Response.WriteAsync(jsonString);
        }

        private async Task<string> FormatResponse(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var plainBodyText = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);

            return plainBodyText;
        }

        private bool IsSwagger(HttpContext context)
        {
            return context.Request.Path.StartsWithSegments("/swagger");
        }
    }
}
