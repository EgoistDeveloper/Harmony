using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Harmony.Models.Track
{
    public class ArtistItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Artist Artist { get; set; }
        public ObservableCollection<TrackItem> TrackItems { get; set; } = new ObservableCollection<TrackItem>();
        public ObservableCollection<AlbumItem> AlbumItems { get; set; } = new ObservableCollection<AlbumItem>();
    }
}