using System.ComponentModel;

namespace Harmony.Models.Track
{
    public class Genre : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public long Id { get; set; }
        public string GenreName { get; set; }
    }
}