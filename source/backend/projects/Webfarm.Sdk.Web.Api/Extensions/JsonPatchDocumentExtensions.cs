namespace Webfarm.Sdk.Web.Api.Extensions
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using JetBrains.Annotations;
    using Microsoft.AspNetCore.JsonPatch;

    public static class JsonPatchDocumentExtensions
    {
        public static bool HasOperationFor<T>([NotNull] this JsonPatchDocument<T> patchDoc, string path)
            where T : class
        {
            Contract.Assert(patchDoc != null);
            var hasChanges = patchDoc.Operations.Any(o => o.path.Equals(path, StringComparison.OrdinalIgnoreCase));

            return hasChanges;
        }
    }
}
