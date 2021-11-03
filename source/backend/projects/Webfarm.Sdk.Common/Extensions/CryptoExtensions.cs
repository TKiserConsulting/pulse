namespace Infoplus.Askod.Sdk.Common.Extensions
{
    using System.Security.Cryptography;
    using System.Text;

    public static class CryptoExtensions
    {
        public static string Hash(this byte[] content)
        {
            string hash;
            using (var md5Hash = MD5.Create())
            {
                // Convert the input string to a byte array and compute the hash.
                var data = md5Hash.ComputeHash(content);

                // hash = Encoding.UTF8.GetString(data);
                var builder = new StringBuilder();
                foreach (var h in data)
                {
                    builder.Append(h.ToString("x2"));
                }

                hash = builder.ToString();
            }

            return hash;
        }
    }
}
