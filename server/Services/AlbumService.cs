using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Options;
using PhotoAlbum.Models;
using PhotoAlbum.Models.Options;

namespace PhotoAlbum.Services
{
    public class AlbumService
    {
        private AlbumSettings _settings;
        private PathService _path;
        private Dictionary<string, Album> _albumCache;
        private Album[] _rootAlbums = new Album[0];

        public AlbumService(IOptions<AlbumSettings> options, PathService path)
        {
            _settings = options.Value;
            _path = path;
            _albumCache = new Dictionary<string, Album>();
        }

        public Album[] LoadAlbums()
        {
            _rootAlbums = _settings.AlbumRootPaths
                .Select(a => a.AsDirectoryPath())
                .Select(GetAlbumAtDirectory)
                .ToArray();
            return _rootAlbums;
        }

        public Album[] GetRootAlbums()
        {
            if ((_rootAlbums?.Length ?? 0) < 1)
                LoadAlbums();
            return _rootAlbums;
        }

        public Album GetAlbum(string hash) => _albumCache.SafeGet(hash);

        public void RenameImage(string imageHash, string albumHash, string newName)
        {
            _path.UpdatePath(imageHash, newName);
            UpdateAlbumImages(albumHash);
        }

        private void UpdateAlbumImages(string hash)
        {
            var album = _albumCache.SafeGet(hash);
            if (album == null) return;
            album.Images = GetImagesInDirectory(album.Path.AsDirectoryPath());
        }

        private Album GetAlbumAtDirectory(DirectoryInfo directory)
        {
            var album = new Album { Path = directory.FullName };
            album.Images = GetImagesInDirectory(directory);
            var albums = directory
                .GetDirectories()
                .Select(GetAlbumAtDirectory)
                .Where(a => !a.IsEmpty);
            album.Albums = albums.Select(a => a.ToAbbreviation()).ToArray();
            if (!album.IsEmpty)
                _albumCache.Add(album.Hash, album);
            return album;
        }

        private Image[] GetImagesInDirectory(DirectoryInfo directory)
        {
            var results = directory
                .GetFiles()
                .Where(_path.IsPathAllowed)
                .Where(_path.IsFileAnImage)
                .Select(file => new Image { Path = file.FullName })
                .ToArray();

            foreach(var image in results)
                _path.AddPath(image.Path);
            
            return results;
        }
    }
}