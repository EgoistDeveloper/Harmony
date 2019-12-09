using System.ComponentModel;

namespace Harmony.Models.Track
{
    public class TrackGenre : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public long Id { get; set; }
        public long TrackId { get; set; }
        public long GenreId { get; set; }
    }
}