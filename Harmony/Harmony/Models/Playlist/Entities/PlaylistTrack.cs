using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Harmony.Models.Playlist
{
    public class PlaylistTrack : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public long Id { get; set; }
        [Required]
        public long PlaylistId { get; set; }
        [Required]
        public long TrackId { get; set; }
        public Models.Track.Track Track { get; set; }
    }
}