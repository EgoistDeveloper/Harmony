using GalaSoft.MvvmLight;
using Harmony.Models.Track;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using static Harmony.DI.DI;

namespace Harmony.ViewModel.Playlist
{
    public class PlaylistViewModel : ViewModelBase
    {
        public PlaylistViewModel()
        {
            PlayAsPlaylistCommand = new RelayCommand(p => PlayAsPlaylist());
            RemovePlaylistItemCommand = new RelayParameterizedCommand(RemovePlaylistItem);
        }

        #region Commands

        public ICommand PlayAsPlaylistCommand { get; set; }
        public ICommand RemovePlaylistItemCommand { get; set; }


        #endregion

        #region Command Methods

        public void PlayAsPlaylist()
        {
            if (ViewModelApplication.SelectedPlaylist != null && ViewModelApplication.SelectedPlaylistTrackItems.Count() > 0)
            {
                ViewModelApplication.PlaylistTrackItems = ViewModelApplication.SelectedPlaylistTrackItems;
                ViewModelApplication.PlayTrackItem(ViewModelApplication.SelectedPlaylistTrackItems.First());
            }
        }

        public void RemovePlaylistItem(object sender)
        {
            var trackItem = ((Button)sender).DataContext as TrackItem;

            // play next and remove from list
            if (ViewModelApplication.PlaylistTrackItems.Contains(trackItem))
            {
                var nextTrackItem = ViewModelApplication.GetNextTrackItem(ViewModelApplication.PlaylistTrackItems, trackItem);
                ViewModelApplication.PlayTrackItem(nextTrackItem);

                ViewModelApplication.PlaylistTrackItems.Remove(trackItem);
            }

            ViewModelApplication.SelectedPlaylistTrackItems.Remove(trackItem);

            // remove from vurrent playing
            if (ViewModelApplication.SelectedTrackItem == trackItem)
            {
                ViewModelApplication.NAudioPlayer.Stop();

                ViewModelApplication.SelectedTrackItem = null;
            }
        }

        #endregion
    }
}