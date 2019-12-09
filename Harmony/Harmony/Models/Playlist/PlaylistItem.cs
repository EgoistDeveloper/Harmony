using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Harmony.Models.Playlist
{
    public class PlaylistItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Playlist Playlist { get; set; }
        public ObservableCollection<PlaylistTrackItem> PlaylistTrackItems { get; set; } = 
            new ObservableCollection<PlaylistTrackItem>();
    }
}