namespace Webfarm.Sdk.Web.Api.Providers
{
    using System.Diagnostics.Contracts;
    using System.Linq;
    using JetBrains.Annotations;
    using Microsoft.AspNetCore.Mvc.ApplicationModels;
    using Microsoft.AspNetCore.Mvc.Routing;

    public class RouteConvention : IApplicationModelConvention
    {
        private readonly AttributeRouteModel centralPrefix;

        public RouteConvention(IRouteTemplateProvider routeTemplateProvider)
        {
            this.centralPrefix = new AttributeRouteModel(routeTemplateProvider);
        }

        public void Apply([NotNull] ApplicationModel application)
        {
            Contract.Assert(application != null);

            foreach (var controller in application.Controllers)
            {
                var matchedSelectors = controller.Selectors.Where(x => x.AttributeRouteModel != null).ToList();
                if (matchedSelectors.Any())
                {
                    foreach (var selectorModel in matchedSelectors)
                    {
                        selectorModel.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(
                            this.centralPrefix,
                            selectorModel.AttributeRouteModel);
                    }
                }

                var unmatchedSelectors = controller.Selectors.Where(x => x.AttributeRouteModel == null).ToList();
                if (unmatchedSelectors.Any())
                {
                    foreach (var selectorModel in unmatchedSelectors)
                    {
                        selectorModel.AttributeRouteModel = this.centralPrefix;
                    }
                }
            }
        }
    }
}
