using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Salesgram.Core.Feature
{
    public class ModelStateFeature
    {
        public ModelStateDictionary ModelState { get; set; }

        public ModelStateFeature(ModelStateDictionary state)
        {
            this.ModelState = state;
        }
    }
}
