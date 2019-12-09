using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Media.Imaging;

namespace Harmony.Models.Track
{
    public class Album : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public long Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public long ArtistId { get; set; }
        public string Description { get; set; }
        public DateTime ReleaseDate { get; set; }
        public BitmapImage Image { get; set; }
    }
}