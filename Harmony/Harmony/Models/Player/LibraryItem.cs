using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harmony.Models.Player
{
    public class LibraryItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Library Library { get; set; }
        public DirectoryInfo DirectoryInfo { get; set; }
    }
}
