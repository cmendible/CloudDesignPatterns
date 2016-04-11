namespace ValetKey
{
    using Microsoft.WindowsAzure.Storage.Auth;
    using Microsoft.WindowsAzure.Storage.Blob;
    using Newtonsoft.Json;
    using System;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;

    public class ValetKeyClient
    {
        public static void UploadBlob()
        {
            // Make sure the endpoint matches with the web role's endpoint.
            var tokenServiceEndpoint = "http://localhost:5000/api/sas";

            try
            {
                var blobSas = GetBlobSas(new Uri(tokenServiceEndpoint)).Result;

                // Create storage credentials object based on SAS
                var credentials = new StorageCredentials(blobSas.Credentials);

                // Using the returned SAS credentials and BLOB Uri create a block blob instance to upload
                var blob = new CloudBlockBlob(blobSas.BlobUri, credentials);

                using (var stream = GetFileToUpload(10))
                {
                    blob.UploadFromStream(stream);
                }

                Console.WriteLine();
                Console.WriteLine("Blob uplodad successful: {0}", blobSas.Name);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine();
            Console.WriteLine("Done. Press any key to exit...");
            Console.ReadKey();
        }

        private static async Task<StorageEntitySas> GetBlobSas(Uri blobUri)
        {
            var request = HttpWebRequest.Create(blobUri);
            var response = await request.GetResponseAsync();

            var serializer = new JsonSerializer();
            using (var sr = new StreamReader(response.GetResponseStream()))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                var blobSas = serializer.Deserialize(jsonTextReader, typeof(StorageEntitySas));
                return (StorageEntitySas)blobSas;
            }
        }

        /// <summary>
        /// Create a sample file containing random bytes of data
        /// </summary>
        /// <param name="sizeMb"></param>
        /// <returns></returns>
        private static MemoryStream GetFileToUpload(int sizeMb)
        {
            var stream = new MemoryStream();

            var rnd = new Random();
            var buffer = new byte[1024 * 1024];

            for (int i = 0; i < sizeMb; i++)
            {
                rnd.NextBytes(buffer);
                stream.Write(buffer, 0, buffer.Length);
            }

            stream.Position = 0;

            return stream;
        }

        public struct StorageEntitySas
        {
            public string Credentials;
            public Uri BlobUri;
            public string Name;
        }
    }
}
