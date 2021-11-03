namespace Webfarm.Sdk.Web.Api.Data
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    public class RangeModelBinder : IModelBinder
    {
        public const string RangeDisableTotalHeaderKey = "range-disable-total";

        public string SystemUnitName { get; set; } = "range";

        // http://otac0n.com/blog/2012/11/21/range-header-i-choose-you.html
        public Task BindModelAsync([NotNull] ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            RangeModel model;
            if (!this.TryParse(bindingContext, out model))
            {
                model = new RangeModel { Unit = this.GetUnitName(bindingContext) };
            }

            bindingContext.Result = ModelBindingResult.Success(model);
            return Task.CompletedTask;
        }

        private bool TryParse([NotNull] ModelBindingContext bindingContext, [CanBeNull] out RangeModel model)
        {
            var parsed = false;
            model = null;

            var requestHeaders = bindingContext.HttpContext.Request.GetTypedHeaders();
            var rangeHeaders = requestHeaders.Range;
            var rangeHeader = rangeHeaders?.Ranges.FirstOrDefault();
            if (rangeHeader != null)
            {
                var disableTotal = false;
                if (bindingContext.HttpContext.Request.Headers.ContainsKey(RangeDisableTotalHeaderKey))
                {
                    var val = bindingContext.HttpContext.Request.Headers[RangeDisableTotalHeaderKey].FirstOrDefault();
                    if (val != null && (val == "1" || string.Equals(bool.TrueString, val, StringComparison.OrdinalIgnoreCase)))
                    {
                        disableTotal = true;
                    }
                }

                model = new RangeModel
                {
                    Skip = (int)Math.Max(rangeHeader.From ?? 0, 0),
                    Unit = this.GetUnitName(bindingContext),
                    DisableTotal = disableTotal
                };

                if (rangeHeader.To.HasValue)
                {
                    if (rangeHeader.To.Value >= model.Skip)
                    {
                        model.Take = (int)(rangeHeader.To.Value - model.Skip + 1);
                    }
                    else
                    {
                        model.Take = 0;
                    }
                }

                parsed = true;
            }

            return parsed;
        }

        private string GetUnitName(ModelBindingContext bindingContext) =>
            bindingContext.FieldName.Contains(this.SystemUnitName, StringComparison.OrdinalIgnoreCase)
                ? RangeModel.DefaultUnit
                : bindingContext.FieldName;
    }
}
