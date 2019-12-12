using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Harmony.Models.Playlist
{
    public class DailyMixTrack : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public long Id { get; set; }
        [Required]
        public long DailyMixId { get; set; }
        [Required]
        public long TrackId { get; set; }

        public DailyMix DailyMix { get; set; }
    }
}