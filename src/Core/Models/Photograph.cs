using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public sealed class Photograph
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string FilmUsed { get; set; } = string.Empty;
        public string CameraUsed { get; set; } = string.Empty;
        public DateOnly DateTaken { get; set; }
        public bool IsPublished { get; set; }
        public Location Location { get; set; } = new();
        public PhotoData HdImageData { get; set; } = new();
        public PhotoData SdImageData { get; set; } = new();
    }
}
