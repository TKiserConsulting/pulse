namespace Webfarm.Sdk.Web.Api.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Net.Http.Headers;
    using Webfarm.Sdk.Web.Api.Data;

    public static class RangeExtensions
    {
        [CanBeNull]
        public static IEnumerable<T> Paged<T>(
            [NotNull] this ControllerBase controller,
            IEnumerable<T> items,
            [NotNull] RangeModel range,
            long? totalCount = null)
        {
            Contract.Assert(controller != null);
            Contract.Assert(range != null);

            return controller.Paged(range.Unit, items, range.Skip, totalCount);
        }

        [CanBeNull]
        public static IEnumerable<T> Paged<T>(
            [NotNull] this ControllerBase controller,
            string name,
            IEnumerable<T> items,
            long skip,
            long? totalCount = null)
        {
            Contract.Assert(controller != null);

            if (controller.Request.Headers.ContainsKey(HeaderNames.Range))
            {
                controller.Response.StatusCode = (int)HttpStatusCode.PartialContent;
            }

            return controller.Response.Paged(name, items, skip, totalCount);
        }

        [CanBeNull]
        public static IEnumerable<T> Paged<T>(
            [NotNull] this HttpResponse response,
            string name,
            [CanBeNull] IEnumerable<T> items,
            long skip,
            long? totalCount)
        {
            Contract.Assert(response != null);

            var list = items?.ToArray();

            var headers = response.GetTypedHeaders();

            headers.Append(HeaderNames.AcceptRanges, name);

            var fromIndex = skip;
            var toIndex = Math.Max(fromIndex, fromIndex + (list?.Length ?? 0) - 1);
            var length = totalCount;

            if (length.HasValue && toIndex > length)
            {
                toIndex = Math.Max(length.Value - 1, 0);
            }

            if (fromIndex > toIndex)
            {
                fromIndex = toIndex;
            }

            headers.ContentRange =
                length.HasValue ?
                    new ContentRangeHeaderValue(fromIndex, toIndex, length.Value) { Unit = name } :
                    new ContentRangeHeaderValue(fromIndex, toIndex) { Unit = name };

            return list;
        }

        /*
        public static long? CountTotal([NotNull] this RangeModel range, [NotNull] Func<long> counter)
        {
            Contract.Assert(range != null);
            Contract.Assert(counter != null);
            return range.DisableTotal ? (long?)null : counter();
        }
        */

        public static async Task<long?> CountTotal([NotNull] this RangeModel range, [NotNull] Func<CancellationToken, Task<int>> counter, CancellationToken cancellationToken)
        {
            Contract.Assert(range != null);
            Contract.Assert(counter != null);
            var result = (long?)null;
            if (!range.DisableTotal)
            {
                result = await counter(cancellationToken);
            }

            return result;
        }
    }
}
