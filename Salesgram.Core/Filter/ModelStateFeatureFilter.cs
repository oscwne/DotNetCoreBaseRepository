using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Salesgram.Core.Feature;

namespace Salesgram.Core.Filter
{
    public class ModelStateFeatureFilter
    {
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                var state = context.ModelState;
                context.HttpContext.Features.Set(new ModelStateFeature(state));
            }

            await next();
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
            throw new NotImplementedException();
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            throw new NotImplementedException();
        }
    }
}
