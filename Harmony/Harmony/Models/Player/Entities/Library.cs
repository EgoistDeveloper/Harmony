using System.ComponentModel;

namespace Harmony.Models.Player
{
    public class Library : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public long Id { get; set; }
        public string FolderPath { get; set; }
    }
}