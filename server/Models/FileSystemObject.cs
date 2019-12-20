using System.Text.Json.Serialization;

namespace PhotoAlbum.Models
{
    public abstract class FileSystemObject
    {
        [JsonIgnore] public string Path { get; set; }
        public string Hash => Path.HashMD5();
    }
}