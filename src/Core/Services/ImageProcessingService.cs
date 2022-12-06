using Microsoft.VisualBasic.FileIO;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public sealed class ImageProcessingService
    {
        private readonly int _hdSize = 2560, _sdSize = 720;
        private const long maxFileSize = 600 * 1000000;

        private readonly AzureBlobFileService _fileService;

        public ImageProcessingService(AzureBlobFileService fileService)
        {
            _fileService = fileService;
        }

        public async Task<ImageDataResult> SaveSourceImage(
            Stream sourceStream, int maxWidthPixels, bool useWatermark = false)
        {
            sourceStream.Position = 0;

            (Image image, IImageFormat format) loadImage = await Image.LoadWithFormatAsync(sourceStream);

            using (var source = loadImage.image)
            {
                if (loadImage.format is not JpegFormat)
                {
                    throw new Exception("Image is not JPEG");
                }

                int newHeight = 0, newWidth = 0;

                if (Math.Max(source.Width, source.Height) > maxWidthPixels)
                {
                    newWidth = source.Width > source.Height ?
                        maxWidthPixels : (int)MathF.Floor(
                            ((float)source.Width / (float)source.Height) * maxWidthPixels);

                    newHeight = source.Width < source.Height ?
                        maxWidthPixels : (int)MathF.Floor(
                            ((float)source.Height / (float)source.Width) * maxWidthPixels);
                }
                else
                {
                    newWidth = source.Width;
                    newHeight = source.Height;
                }

                IImageEncoder encoder = new JpegEncoder()
                {
                    Quality = 100
                };

                var fileType = "image/jpeg";

                source.Mutate(i => i.Resize(newWidth, newHeight));

                if (useWatermark)
                {
                    int marksPerRow = 2;
                    int markRows = 3;

                    FontFamily ff = new FontCollection().Add("./wwwroot/arial.ttf");

                    string watermarkText = "antonljunggren.se";
                    var fontSize = 130f;

                    if (newWidth < _hdSize)
                    {
                        fontSize = 100f;
                    }

                    Font font = ff.CreateFont((int)Math.Floor(fontSize), FontStyle.BoldItalic);

                    TextOptions opt = new(font)
                    {
                        Origin = new PointF(0, 0),
                        TabWidth = 4,
                        WrappingLength = 500,
                        HorizontalAlignment = HorizontalAlignment.Left
                    };

                    Color color = new Color(new Vector4(1, 1, 1, 0.5f));
                    var rand = new Random();

                    for (int y = 0; y < markRows; y++)
                    {
                        for (int x = 0; x < marksPerRow; x++)
                        {
                            opt.Origin = new PointF(newWidth / marksPerRow * x + rand.Next(0, 50),
                                newHeight / markRows * y + (newHeight / markRows) / 2);

                            source.Mutate(i => i.DrawText(opt, watermarkText, color));
                        }
                    }
                }

                using Stream outStream = new MemoryStream();
                await source.SaveAsync(outStream, encoder);

                outStream.Position = 0;

                int r = 0;
                var buf = new byte[outStream.Length];
                while ((r = outStream.Read(buf, 0, buf.Length)) > 0)
                {

                }

                var location = await _fileService.SaveFile(outStream, fileType);
                return new ImageDataResult(location, fileType);
            }

        }
    }

    public struct ImageDataResult
    {
        public ImageDataResult(string imageLocation, string contentType)
        {
            ImageLocation = imageLocation;
            ContentType = contentType;
        }

        public readonly string ImageLocation;
        public readonly string ContentType;
    } 
}
