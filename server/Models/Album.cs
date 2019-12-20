using System.Linq;
using System.Text.Json.Serialization;

namespace PhotoAlbum.Models
{

    public class Album : FileSystemObject
    {
        public Image[] Images { get; set; } = new Image[0];
        public AbbreviatedAlbum[] Albums { get; set; } = new AbbreviatedAlbum[0];
        [JsonIgnore] public AlbumInfo Info { get; set; }

        [JsonIgnore] public bool IsEmpty => !(Albums?.Any() ?? false) && !(Images?.Any() ?? false);

        public string Name => Info?.Name ?? Path.AsDirectoryPath().Name;
        public string CoverImageHash => Info?.CoverImageHash ?? (Images?.Any() ?? false ? Images[0].Hash : null);

        public AbbreviatedAlbum ToAbbreviation() => new AbbreviatedAlbum
        {
            Name = Name,
            Hash = Hash,
            Images = Images.Length
        };
    }
}