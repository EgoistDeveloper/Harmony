using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harmony.Models.Track
{
    public class AlbumItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Album Album { get; set; }
        public ObservableCollection<TrackItem> TrackItems { get; set; } = new ObservableCollection<TrackItem>();
        public ObservableCollection<Artist> Artists { get; set; } = new ObservableCollection<Artist>();
    }
}
