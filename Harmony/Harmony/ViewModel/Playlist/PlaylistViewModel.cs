﻿using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static Harmony.DI.DI;

namespace Harmony.ViewModel.Playlist
{
    public class PlaylistViewModel : ViewModelBase
    {
        public PlaylistViewModel()
        {
            PlayAsPlaylistCommand = new RelayCommand(p => PlayAsPlaylist());

        }

        #region Commands

        public ICommand PlayAsPlaylistCommand { get; set; }

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

        #endregion
    }
}