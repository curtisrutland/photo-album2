namespace PhotoAlbum.Models
{
    public class Image : FileSystemObject
    {
        public string Name => Path.AsFilePath().NameWithoutExtension();

    }
}