namespace Webfarm.Sdk.Web.Api.Data
{
    using JetBrains.Annotations;
    using Microsoft.AspNetCore.Mvc;
    using Webfarm.Sdk.Data;

    [ModelBinder(BinderType = typeof(RangeModelBinder))]
    public class RangeModel : IRange
    {
        public const int PageSize = 10;

        public const string DefaultUnit = "x-items";

        private string unit;

        public int Skip { get; set; }

        public int Take { get; set; } = PageSize;

        public bool DisableTotal { get; set; }

        [NotNull]
        public string Unit
        {
            get => string.IsNullOrEmpty(this.unit) ? DefaultUnit : this.unit;
            set => this.unit = value;
        }
    }
}
