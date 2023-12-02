using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Zabr.Crawler.Common.Extensions
{
    public static class CommonExtensions
    {
        public static async Task<T?> GetObjectAsync<T>(this byte[] obj, CancellationToken stoppingToken)
        {
            using (var stream = new MemoryStream(obj))
            {
                var result = await JsonSerializer.DeserializeAsync<T>(stream, new JsonSerializerOptions(JsonSerializerDefaults.General), stoppingToken);
                return result;
            }
        }

        public static async Task<string> GetJsonAsync(this object obj, CancellationToken stoppingToken)
        {
            using (var stream = new MemoryStream())
            {
                await JsonSerializer.SerializeAsync(stream, obj, obj.GetType(),
                    new JsonSerializerOptions(JsonSerializerDefaults.General), stoppingToken);

                stream.Position = 0;

                using var reader = new StreamReader(stream);
                return await reader.ReadToEndAsync();
            }
        }

        public static async Task<string> GetHashAsync(this string obj, CancellationToken stoppingToken)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            return await Task.Run(() =>
            {
                using (var sha256 = SHA256.Create())
                {
                    // Convert the input string to a byte array and compute the hash
                    byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(obj));

                    // Convert the byte array to a hexadecimal string
                    var builder = new StringBuilder();
                    foreach (var b in bytes)
                    {
                        builder.Append(b.ToString("x2"));
                    }

                    return builder.ToString();
                }
            }, stoppingToken);
        }

        public static string GetHash(this string obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            using (var sha256 = SHA256.Create())
            {
                // Convert the input string to a byte array and compute the hash
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(obj));

                // Convert the byte array to a hexadecimal string
                var builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }

                return builder.ToString();
            }
        }
    }
}
