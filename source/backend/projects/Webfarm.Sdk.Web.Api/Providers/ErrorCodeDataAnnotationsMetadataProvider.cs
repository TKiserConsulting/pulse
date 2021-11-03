namespace Webfarm.Sdk.Web.Api.Providers
{
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.Contracts;
    using JetBrains.Annotations;
    using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

    public class ErrorCodeDataAnnotationsMetadataProvider : IBindingMetadataProvider, IDisplayMetadataProvider, IValidationMetadataProvider
    {
        public void CreateBindingMetadata(BindingMetadataProviderContext context)
        {
        }

        public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
        {
        }

        public void CreateValidationMetadata([NotNull] ValidationMetadataProviderContext context)
        {
            Contract.Assert(context != null);

            foreach (var metadata in context.ValidationMetadata.ValidatorMetadata)
            {
                if (metadata is ValidationAttribute validationAttribute)
                {
                    var code = validationAttribute.GetType().Name;
                    var message = validationAttribute.FormatErrorMessage("{0}");
                    validationAttribute.ErrorMessage =
                        $"[{code}] {message}";
                }
            }
        }
    }
}
