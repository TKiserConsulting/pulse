namespace Webfarm.Sdk.Web.Api.Data
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    public class FilterModelBinder : IModelBinder
    {
        public Task BindModelAsync([NotNull] ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var dict = new FilteringData();
            foreach (var key in bindingContext.HttpContext.Request.Query.Keys)
            {
                if (key.Contains(bindingContext.FieldName, StringComparison.InvariantCultureIgnoreCase))
                {
                    var val = bindingContext.ValueProvider.GetValue(key);
                    if (val.Any())
                    {
                        var newKey = Regex.Replace(
                            key,
                            bindingContext.FieldName + @"|\[|\]",
                            string.Empty,
                            RegexOptions.IgnoreCase);
                        dict.Add(newKey, val.FirstValue);
                    }
                }
            }

            bindingContext.Result = ModelBindingResult.Success(dict);

            return Task.CompletedTask;
        }
    }
}
