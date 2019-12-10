using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Harmony.Models.Track
{
    public class Plays : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Plays()
        {
            AddedDate = DateTime.Now.Date;
            PlayCount = 1;
        }

        public long Id { get; set; }
        [Required]
        public DateTime AddedDate { get; set; }
        [Required]
        public long TrackId { get; set; }
        [Required]
        public int PlayCount { get; set; }
    }
}