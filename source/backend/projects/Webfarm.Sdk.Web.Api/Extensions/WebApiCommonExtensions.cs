using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Infoplus.Askod.Sdk.Web.Api.Extensions
{
    public static class WebApiCommonExtensions
    {
        public static async Task<byte[]> Convert(this IFormFile file)
        {
            byte[] fileContent;
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);

                fileContent = stream.ToArray();
            }

            return fileContent;
        }
    }
}