using Core.Models;
using Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Web.Models;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly PhotographService _photographService;
        private readonly AzureBlobFileService _blobFileService;
        private ImageProcessingService _imageProcessing;

        public HomeController(ILogger<HomeController> logger, PhotographService photographService, AzureBlobFileService blobFileService, ImageProcessingService imageProcessing)
        {
            _logger = logger;
            _photographService = photographService;
            _blobFileService = blobFileService;
            _imageProcessing = imageProcessing;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new HomeViewModel();

            var photographs = await _photographService.GetAll();

            viewModel.photographs = photographs;

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetPreview(string? photoId, bool sdImage = true)
        {

            if (string.IsNullOrWhiteSpace(photoId))
                return BadRequest();

            var photo = await _photographService.Get(photoId);
            var photoData = sdImage ? photo.SdImageData : photo.HdImageData;

            var imageStream = await _blobFileService.GetFileContentStream(photoData.ImageFilePath);

            return new FileStreamResult(imageStream, photoData.FileContentType);
        }

        public async Task<IActionResult> EditPhotograph(string? photoId)
        {
            if (string.IsNullOrWhiteSpace(photoId))
                return BadRequest();

            var photo = await _photographService.Get(photoId);

            return View(photo);
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePhotograph(PhotographFormModel formModel)
        {
            var updatePhoto = await _photographService.Get(formModel.Id);
            updatePhoto.Title = formModel.Title;
            updatePhoto.Description = formModel.Description;
            updatePhoto.FilmUsed = formModel.FilmUsed;
            updatePhoto.CameraUsed = formModel.CameraUsed;
            updatePhoto.Location = new Location() 
                { 
                    Country = formModel.Country, 
                    City= formModel.City, 
                    Province = formModel.Province
                };
            updatePhoto.IsPublished = formModel.Published;
            updatePhoto.DateTaken = DateOnly.FromDateTime(formModel.DateTaken);

            if(formModel.ImageFile!= null && formModel.ImageFile.Length > 0)
            {
                using Stream stream = formModel.ImageFile.OpenReadStream();
                var hdData = await _imageProcessing.SaveSourceImage(stream, 2560, true);
                var sdData = await _imageProcessing.SaveSourceImage(stream, 720);

                stream.Close();
                stream.Dispose();

                updatePhoto.HdImageData = new PhotoData
                {
                    FileContentType = hdData.ContentType,
                    ImageFilePath = hdData.ImageLocation
                };

                updatePhoto.SdImageData = new PhotoData
                {
                    FileContentType = sdData.ContentType,
                    ImageFilePath = sdData.ImageLocation
                };
            }

            await _photographService.UpdatePhotograph(updatePhoto);

            return RedirectToAction(nameof(EditPhotograph), new { photoId = formModel.Id });
        }

        [HttpPost]
        public async Task<IActionResult> DeletePhotograph(string? photoId)
        {
            if (string.IsNullOrWhiteSpace(photoId))
            {
                return BadRequest($"Wrong photo id: {photoId?.ToString()}");
            }

            await _photographService.DeletePhotograph(photoId);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotograph(PhotographFormModel formModel)
        {
            if(formModel.ImageFile == null || formModel.ImageFile.Length < 10)
            {
                return BadRequest("Image missing");
            }

            using Stream stream = formModel.ImageFile.OpenReadStream();
            var hdData = await _imageProcessing.SaveSourceImage(stream, 2560, true);
            var sdData = await _imageProcessing.SaveSourceImage(stream, 720);

            stream.Close();
            stream.Dispose();

            var hdImageData = new PhotoData
            {
                FileContentType = hdData.ContentType,
                ImageFilePath = hdData.ImageLocation
            };

            var sdImageData = new PhotoData
            {
                FileContentType = sdData.ContentType,
                ImageFilePath = sdData.ImageLocation
            };

            var photograph = new Photograph()
            {
                Title = formModel.Title,
                Description= formModel.Description,
                FilmUsed= formModel.FilmUsed,
                CameraUsed= formModel.CameraUsed,
                DateTaken = DateOnly.FromDateTime(formModel.DateTaken),
                IsPublished = false,
                HdImageData= hdImageData,
                SdImageData= sdImageData,
                Location = new Location()
                {
                    Country = formModel.Country,
                    City = formModel.City,
                    Province = formModel.Province
                }
            };

            var newPhoto = await _photographService.AddPhotograph(photograph);

            return RedirectToAction(nameof(EditPhotograph), new { photoId = newPhoto.Id });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}