namespace Webfarm.Sdk.Web.Api.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Webfarm.Sdk.Data.Exceptions;
    using Webfarm.Sdk.Data.Metadata;

    public class SortModelBinder<T> : IModelBinder
    {
        public Task BindModelAsync([NotNull] ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var sortByValues = bindingContext.ValueProvider.GetValue(bindingContext.FieldName);

            var model = this.CheckSortParam(sortByValues.Values);

            bindingContext.Result = ModelBindingResult.Success(model);

            return Task.CompletedTask;
        }

        private string[] CheckSortParam([CanBeNull] string[] incomingValues)
        {
            var ret = new List<string>();
            if (incomingValues != null && incomingValues.Any())
            {
                if (incomingValues.Length == 1 && incomingValues.Single().Equals("null", StringComparison.OrdinalIgnoreCase))
                {
                    return ret.ToArray();
                }

                var errors = new List<ApplicationValidationResult>();
                var properties = typeof(T).GetProperties();
                foreach (var s in incomingValues)
                {
                    var arr = s.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (arr.Length == 0)
                    {
                        continue;
                    }

                    var propName = arr[0];
                    var sortDir = arr.Length > 1 ? arr[1] : string.Empty;
                    if (!string.IsNullOrEmpty(propName)
                        && properties.Any(p => p.Name.ToUpperInvariant() == propName.ToUpperInvariant()))
                    {
                        var sortByProp = propName;
                        if (!string.IsNullOrEmpty(sortDir) && sortDir.Equals("desc", StringComparison.OrdinalIgnoreCase))
                        {
                            sortByProp = $"{sortByProp} desc";
                        }

                        ret.Add(sortByProp);
                    }
                    else
                    {
                        errors.Add(new ApplicationValidationResult(
                            propName,
                            WellKnownErrorSubcodeTypes.Invalid,
                            "Property not exists"));
                    }
                }

                if (errors.Any())
                {
                    throw new ApplicationValidationException(errors);
                }
            }

            return ret.ToArray();
        }
    }
}
