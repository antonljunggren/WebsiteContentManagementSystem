using Core.Models;

namespace Web.Models
{
    public sealed class HomeViewModel
    {
        public List<Photograph> photographs { get; set; } = new ();
    }
}
