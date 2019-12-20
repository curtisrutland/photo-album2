using Microsoft.AspNetCore.Mvc;
using PhotoAlbum.Services;

namespace PhotoAlbum.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImagesController : ControllerBase
    {
        private PathService _path;

        public ImagesController(PathService path) => _path = path;

        [HttpGet("{hash}")]
        public IActionResult Get(string hash)
        {
            var path = _path.GetPath(hash);
            if(string.IsNullOrWhiteSpace(path))
                return NotFound();
            var file = path.AsFilePath();
            if(!file.Exists)
                return NotFound();
            var mimeType = file.GetMimeType();
            if(string.IsNullOrWhiteSpace(mimeType) || !mimeType.Contains("image"))
                return StatusCode(500, "Invalid file type");
            return File(file.ReadAllBytes(), mimeType);
        }
    }
}