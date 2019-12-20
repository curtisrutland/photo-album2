using Microsoft.AspNetCore.Mvc;
using PhotoAlbum.Models;
using PhotoAlbum.Services;

namespace PhotoAlbum.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AlbumsController : ControllerBase
    {
        private AlbumService _service;

        public AlbumsController(AlbumService service) => _service = service;

        [HttpGet]
        public Album[] GetRootAlbums() => _service.GetRootAlbums();

        [HttpPatch("refresh")]
        public IActionResult RefreshAlbums()
        {
            _service.LoadAlbums();
            return Ok();
        }

        [HttpGet("{hash}")]
        public IActionResult GetAlbum(string hash)
        {
            var album = _service.GetAlbum(hash);
            if (album == null)
                return NotFound();
            return Ok(album);
        }



        [HttpPatch("{albumHash}")]
        public IActionResult Patch(string albumHash, [FromBody] Image newImageDetails)
        {
            _service.RenameImage(newImageDetails.Hash, albumHash, newImageDetails.Name);
            return Ok();
        }
    }
}