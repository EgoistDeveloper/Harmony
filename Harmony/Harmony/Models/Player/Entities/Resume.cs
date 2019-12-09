using System.ComponentModel;

namespace Harmony.Models.Player
{
    public class Resume : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public long Id { get; set; }
        public string CurrentVolume { get; set; }
        public double CurrentPosition { get; set; }

        public bool IsPlayingAlbum { get; set; }
        public bool PlayingAlbumId { get; set; }

        public bool IsPlayingPlaylist { get; set; }
        public bool PlayingPlaylistId { get; set; }

        public bool IsPlayingDailyMix { get; set; }
        public bool PlayingDailyMixId { get; set; }

        public bool IsPlayingTrack { get; set; }
        public bool PlayingTrackId { get; set; }
    }
}