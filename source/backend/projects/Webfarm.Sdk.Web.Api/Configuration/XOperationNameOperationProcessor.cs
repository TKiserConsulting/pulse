namespace Webfarm.Sdk.Web.Api.Configuration
{
    using System.Collections.Generic;
    using NSwag.Generation.AspNetCore;
    using NSwag.Generation.Processors;
    using NSwag.Generation.Processors.Contexts;

    public class XOperationNameOperationProcessor : IOperationProcessor
    {
        public bool Process(OperationProcessorContext context)
        {
            var operation = context.OperationDescription.Operation;
            if (operation.ExtensionData == null)
            {
                operation.ExtensionData = new Dictionary<string, object>();
            }

            const string key = "x-operation-name";
            if (!operation.ExtensionData.ContainsKey(key))
            {
                if (context is AspNetCoreOperationProcessorContext swaggerContext)
                {
                    var value = ToCamelCase(swaggerContext.MethodInfo.Name);
                    operation.ExtensionData[key] = value;
                }
            }

            return true;
        }

        private static string ToCamelCase(string str)
        {
            if (!string.IsNullOrEmpty(str) && str.Length > 1)
            {
                return char.ToLowerInvariant(str[0]) + str.Substring(1);
            }

            return str;
        }
    }
}
