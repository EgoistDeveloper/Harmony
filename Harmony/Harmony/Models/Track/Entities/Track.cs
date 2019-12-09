using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Media.Imaging;

namespace Harmony.Models.Track
{
    public class Track : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public long Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string FileLocation { get; set; }
        public TimeSpan Time { get; set; }
        public BitmapImage Image { get; set; }
        public string Description { get; set; }
        public string Lyrics { get; set; }

        public uint Disc { get; set; }
        public uint Number { get; set; }

        public long AlbumId { get; set; }
        public long ArtistId { get; set; }
    }
}