using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Harmony.Models.Track
{
    public class AlbumArtist : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public long Id { get; set; }
        [Required]
        public long AlbumId { get; set; }
        [Required]
        public long ArtistId { get; set; }
    }
}
