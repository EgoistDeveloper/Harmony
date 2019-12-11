using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Harmony.Models.Playlist
{
    public class DailyMix : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public DailyMix()
        {
            AddedDate = DateTime.Now.Date;
        }

        public long Id { get; set; }
        [Required]
        public DateTime AddedDate { get; set; }
    }
}