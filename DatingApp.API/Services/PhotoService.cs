using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.API.Helpers;
using DatingApp.API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace DatingApp.API.Services
{
    public class PhotoService : IPhotoService
    {
        private Cloudinary cloudinary;

        public PhotoService(IOptions<CloudinarySettings> options)
        {
            var cloudinaryAccount = new Account(options.Value.CloudName, 
                                                options.Value.ApiKey, 
                                                options.Value.ApiSecret);

            cloudinary = new Cloudinary(cloudinaryAccount);
        }

        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
            if(file.Length <= 0)
            {
                return new ImageUploadResult();
            }

            using var stream = file.OpenReadStream();

            var parameters = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, stream),
                Transformation = new Transformation()
                                        .Height(500)
                                        .Width(500)
                                        .Crop("fill")
                                        .Gravity(Gravity.Face)
            };

            return await cloudinary.UploadAsync(parameters);
        }

        public async Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
            return await cloudinary.DestroyAsync(new DeletionParams(publicId));
        }
    }
}
