using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Harmony.Models.Track
{
    public class PlaysItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public int Order { get; set; }
        public string TimeAgo { get; set; }
        public Track Track { get; set; }
        public Plays Plays { get; set; }
    }
}