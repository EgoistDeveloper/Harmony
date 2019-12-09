using System.ComponentModel;

namespace Harmony.Models.Playlist
{
    public class PlaylistTrackItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public PlaylistTrack PlaylistTrack { get; set; }
        public Track.Track Track { get; set; }
    }
}