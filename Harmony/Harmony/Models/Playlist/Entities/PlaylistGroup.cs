using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Harmony.Models.Playlist
{
    public class PlaylistGroup : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public long Id { get; set; }
        [Required]
        public string Title { get; set; }
    }
}