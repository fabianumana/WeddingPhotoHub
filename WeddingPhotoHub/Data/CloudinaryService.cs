using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace WeddingPhotoHub.Data
{
    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IConfiguration config)
        {
            var account = new Account(
                config["Cloudinary:CloudName"],
                config["Cloudinary:ApiKey"],
                config["Cloudinary:ApiSecret"]
            );

            _cloudinary = new Cloudinary(account);
        }

        // 🔥 UPLOAD
        public async Task<(string? Url, string? PublicId)> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return (null, null);

            await using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(Guid.NewGuid().ToString(), stream),
                Folder = "weddingphotohub",
                Transformation = new Transformation()
                    .Quality(80)
                    .FetchFormat("auto")
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result?.SecureUrl == null || string.IsNullOrWhiteSpace(result.PublicId))
            {
                return (null, null);
            }

            return (
                result.SecureUrl.ToString(),
                result.PublicId
            );
        }

        // 🗑 DELETE
        public void DeleteImage(string publicId)
        {
            if (string.IsNullOrWhiteSpace(publicId))
                return;

            var deletionParams = new DeletionParams(publicId);
            var result = _cloudinary.Destroy(deletionParams);

            if (result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                // opcional: log aquí
                Console.WriteLine("Error eliminando imagen en Cloudinary");
            }
        }
    }
}
