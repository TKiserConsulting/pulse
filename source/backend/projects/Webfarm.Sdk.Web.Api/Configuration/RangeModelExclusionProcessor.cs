namespace Webfarm.Sdk.Web.Api.Configuration
{
    using System;
    using System.Linq;
    using NJsonSchema;
    using NSwag;
    using NSwag.Generation.AspNetCore;
    using NSwag.Generation.Processors;
    using NSwag.Generation.Processors.Contexts;
    using Webfarm.Sdk.Web.Api.Data;

    public class RangeModelExclusionProcessor : IOperationProcessor
    {
        public bool Process(OperationProcessorContext context)
        {
            if (context is AspNetCoreOperationProcessorContext swaggerContext)
            {
                var deletable = swaggerContext.ApiDescription.ParameterDescriptions
                    .Where(pd =>
                        pd.Type == typeof(RangeModel))
                    .ToArray();
                Array.ForEach(
                    deletable,
                    m =>
                    {
                        swaggerContext.ApiDescription.ParameterDescriptions.Remove(m);

                        var binderModelName = m.ModelMetadata.BinderModelName ?? RangeModel.DefaultUnit;
                        var operation = swaggerContext.OperationDescription.Operation;
                        operation.Parameters.Add(
                            new OpenApiParameter
                            {
                                Name = "range",
                                Kind = OpenApiParameterKind.Header,
                                Default = $"{binderModelName}=0-{RangeModel.PageSize}",
                                IsRequired = false,
                                IsNullableRaw = true,
                                Schema = new JsonSchemaProperty
                                {
                                    Type = JsonObjectType.String
                                }
                            });
                        operation.Parameters.Add(
                            new OpenApiParameter
                            {
                                Name = RangeModelBinder.RangeDisableTotalHeaderKey,
                                Kind = OpenApiParameterKind.Header,
                                Default = "0",
                                IsRequired = false,
                                IsNullableRaw = true,
                                Schema = new JsonSchemaProperty
                                {
                                    Type = JsonObjectType.Boolean
                                }
                            });
                    });
            }

            return true;
        }
    }
}
