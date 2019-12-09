using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Harmony.Models.Playlist
{
    public class PlaylistGroupItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand AddPlayListCommand { get; set; }
        public ICommand PlaylistGroupItemExpandedCommand { get; set; }

        public bool IsExpanded { get; set; }
        public PlaylistGroup PlaylistGroup { get; set; }
        public ObservableCollection<Playlist> Playlists { get; set; } = new ObservableCollection<Playlist>();
    }
}