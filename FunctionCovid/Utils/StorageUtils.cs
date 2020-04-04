using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Threading.Tasks;

namespace Utils
{
    public class StorageUtils
    {
        private readonly CloudStorageAccount cloudStorageAccount;
        private readonly Uri blobUri;
        private readonly string containerName;
        public StorageUtils(string connectionString)
        {
            if (!connectionString.StartsWith("http"))
            {
                cloudStorageAccount = CreateStorageAccountFromConnectionString(connectionString);
                blobUri = null;
                containerName = null;
            }
            else
            {
                cloudStorageAccount = null;
                blobUri = new Uri(connectionString);
                if (string.IsNullOrEmpty(blobUri.AbsolutePath) || blobUri.AbsolutePath.Equals("/"))
                    containerName = null;
                else
                    containerName = blobUri.AbsolutePath.Replace("/", "");
            }
        }

        public CloudStorageAccount CreateStorageAccountFromConnectionString(string storageConnectionString)
        {
            CloudStorageAccount storageAccount;
            storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            return storageAccount;
        }

        public async Task InsertBlobAsync(string blobName, byte[] data, string contentType = null)
        {
            var cloudBlobContainer = new CloudBlobContainer(blobUri);
            var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(blobName);
            if (!string.IsNullOrEmpty(contentType))
                cloudBlockBlob.Properties.ContentType = contentType;
            await cloudBlockBlob.UploadFromByteArrayAsync(data, 0, data.Length);
        }
    }
}
