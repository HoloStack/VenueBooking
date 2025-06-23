using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace venueBooking.Services
{
    public class BlobService
    {
        private readonly BlobContainerClient _container;
        private readonly string _accountName;
        private readonly string _accountKey;

        public BlobService(IConfiguration config)
        {
            var conn = config["AzureBlob:ConnectionString"];
            var name = config["AzureBlob:ContainerName"];
            if (string.IsNullOrWhiteSpace(conn) || string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Missing AzureBlob configuration.");

            // Parse account name/key to enable SAS generation
            var connStringParts = conn.Split(';');
            _accountName = connStringParts.FirstOrDefault(p => p.StartsWith("AccountName="))?.Split('=')[1] ?? throw new ArgumentException("Invalid connection string: missing AccountName");
            _accountKey = connStringParts.FirstOrDefault(p => p.StartsWith("AccountKey="))?.Split('=')[1] ?? throw new ArgumentException("Invalid connection string: missing AccountKey");

            _container = new BlobContainerClient(conn, name);
            // Ensure container exists; skip creation if service policy forbids public access
            try
            {
                _container.CreateIfNotExists();
            }
            catch (Azure.RequestFailedException ex) when (ex.ErrorCode == "PublicAccessNotPermitted")
            {
                // Container likely exists or account disallows creation; ignore
            }
        }
         public async Task<string> UploadAsync(IFormFile file)
    {
        // Example implementation: Save file to local storage and return the path
        // Replace this with your actual blob storage logic
        var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
        if (!Directory.Exists(uploads))
        {
            Directory.CreateDirectory(uploads);
        }
        var filePath = Path.Combine(uploads, file.FileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }
        // Return a relative path or URL as needed
        return "/uploads/" + file.FileName;
    }
    }
    
}