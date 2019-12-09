using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Media.Imaging;

namespace Harmony.Models.Track
{
    public class Artist : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public long Id { get; set; }
        [Required]
        public string FullName { get; set; }
        public BitmapImage Image { get; set; }
        public BitmapImage Cover { get; set; }
        public string Twitter { get; set; }
        public string Website { get; set; }
    }
}