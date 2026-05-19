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
        public async Task<(string? Url, string? PublicId)> UploadMediaAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return (null, null);

            await using var stream = file.OpenReadStream();

            var extension = Path.GetExtension(file.FileName).ToLower();

            bool esVideo =
                extension == ".mp4" ||
                extension == ".mov" ||
                extension == ".webm";

            RawUploadResult result;

            if (esVideo)
            {
                var uploadParams = new VideoUploadParams
                {
                    File = new FileDescription(Guid.NewGuid().ToString(), stream),
                    Folder = "weddingphotohub/videos"
                };

                result = await _cloudinary.UploadAsync(uploadParams);
            }
            else
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(Guid.NewGuid().ToString(), stream),
                    Folder = "weddingphotohub/images",
                    Transformation = new Transformation()
                        .Quality(80)
                        .FetchFormat("auto")
                };

                result = await _cloudinary.UploadAsync(uploadParams);
            }

            if (result?.SecureUrl == null ||
                string.IsNullOrWhiteSpace(result.PublicId))
            {
                return (null, null);
            }

            return (
                result.SecureUrl.ToString(),
                result.PublicId
            );
        }

        // 🗑 DELETE MEDIA
        public void DeleteMedia(string publicId, string? contentType)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(publicId))
                    return;

                DeletionParams deletionParams;

                if (!string.IsNullOrWhiteSpace(contentType) &&
                    contentType.StartsWith("video"))
                {
                    deletionParams = new DeletionParams(publicId)
                    {
                        ResourceType = ResourceType.Video
                    };
                }
                else
                {
                    deletionParams = new DeletionParams(publicId)
                    {
                        ResourceType = ResourceType.Image
                    };
                }

                var result = _cloudinary.Destroy(deletionParams);

                Console.WriteLine($"Cloudinary delete result: {result.Result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error eliminando archivo: {ex.Message}");
            }
        }
    }
}
