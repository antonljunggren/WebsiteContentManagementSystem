using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Core.Services;
using Microsoft.AspNetCore.Routing;
using Core.Models;

namespace Functions
{
    public sealed class PhotographFunctions
    {
        private readonly AzureBlobFileService _fileService;
        private readonly PhotographService _photographService;

        public PhotographFunctions(AzureBlobFileService fileService, PhotographService photographService)
        {
            _fileService = fileService;
            _photographService = photographService;
        }

        [FunctionName("GetPhotoImage")]
        public async Task<IActionResult> GetPhotoImage(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "photo/{id}/image/{*sd}")] HttpRequest req,
            ILogger log, string id, string sd = "true")
        {

            try
            {
                sd ??= "true";
                var getSd = bool.Parse(sd);

                var photo = await _photographService.Get(id);

                var imageData = getSd ? photo.SdImageData : photo.HdImageData;

                var imageStream = await _fileService.GetFileContentStream(imageData.ImageFilePath);

                return new FileStreamResult(imageStream, imageData.FileContentType);
            }
            catch (NullReferenceException)
            {
                return new OkObjectResult("");
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);

                return new StatusCodeResult(500);
            }
        }

        [FunctionName("GetPhoto")]
        public async Task<IActionResult> GetPhoto(
           [HttpTrigger(AuthorizationLevel.Function, "get", Route = "photo/{id}")] HttpRequest req,
           ILogger log, string id)
        {
            try
            {
                var photo = await _photographService.Get(id);

                return new OkObjectResult(photo);
            }
            catch (NullReferenceException)
            {
                return new OkObjectResult("");
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);

                return new StatusCodeResult(500);
            }
        }

        [FunctionName("GetPhotos")]
        public async Task<IActionResult> GetPhotos(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "photos")] HttpRequest req,
            ILogger log)
        {
            try
            {
                var photos = await _photographService.GetAll();

                return new OkObjectResult(photos);
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);

                return new StatusCodeResult(500);
            }
        }
    }
}
