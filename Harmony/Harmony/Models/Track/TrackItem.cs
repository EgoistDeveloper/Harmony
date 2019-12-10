using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Harmony.Models.Track
{
    public class TrackItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public int Order { get; set; }
        public bool IsPlaying { get; set; }
        public Track Track { get; set; }
        public Artist Artist { get; set; }
        public Album Album { get; set; }
        public ObservableCollection<TrackGenre> TrackGenres { get; set; } = new ObservableCollection<TrackGenre>();
        public ObservableCollection<Plays> Plays { get; set; } = new ObservableCollection<Plays>();
    }
}