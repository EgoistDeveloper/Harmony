using System.ComponentModel;

namespace Harmony.Models.Player
{
    public class PlayerSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public long Id { get; set; }
        public string CurrentVolume { get; set; }
    }
}