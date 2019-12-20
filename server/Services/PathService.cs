using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using PhotoAlbum.Models.Options;

namespace PhotoAlbum.Services
{
    public class PathService
    {
        private readonly string[] _imageExtensions;
        private readonly Regex[] _blacklistedPaths;
        private Dictionary<string, string> _hashesToPaths = new Dictionary<string, string>();
        public string GetPath(string hash) => _hashesToPaths.SafeGet(hash);
        public string AddPath(string path)
        {
            var hash = path.HashMD5();
            _hashesToPaths[hash] = path;
            return hash;
        }

        public PathService(IOptions<AlbumSettings> settings)
        {
            _imageExtensions = settings.Value.ImageExtensions;
            _blacklistedPaths = settings.Value.BlacklistedPaths
              .Select(p => new Regex(p, RegexOptions.Multiline))
              .ToArray();
        }

        public string UpdatePath(string oldHash, string newName)
        {
            var oldPath = _hashesToPaths.SafeGet(oldHash);
            if (string.IsNullOrWhiteSpace(oldPath))
                return null;
            var oldFile = oldPath.AsFilePath();
            var newFullPath = Path.Combine(oldFile.Directory.FullName, $"{newName}{oldFile.Extension}");
            oldFile.MoveTo(newFullPath);
            _hashesToPaths[oldHash] = newFullPath;
            var newHash = newFullPath.HashMD5();
            _hashesToPaths[newHash] = newFullPath;
            return newHash;
        }

        public bool IsFileAnImage(FileSystemInfo file) => file.Extension.EqualsAny(_imageExtensions, true);

        public bool IsPathAllowed(FileSystemInfo file) => !file.FullName.MatchesAny(_blacklistedPaths);
    }
}