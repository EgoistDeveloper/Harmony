using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Media.Imaging;

namespace Harmony.Models.Playlist
{
    public class Playlist : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public long Id { get; set; }
        [Required]
        public DateTime AddedDate { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public BitmapImage Image { get; set; }
        public long PlaylistGroupId { get; set; }
    }
}