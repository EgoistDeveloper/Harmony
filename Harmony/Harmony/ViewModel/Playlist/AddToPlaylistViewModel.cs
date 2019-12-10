using Harmony.Data;
using Harmony.Dialogs.Playlist;
using Harmony.Helpers;
using Harmony.Models.Playlist;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Harmony.ViewModel.Playlist
{
    public class AddToPlaylistViewModel : WindowViewModel
    {
        public AddToPlaylistViewModel(Window window, Models.Track.Track track) : base(window)
        {
            mWindow = window;
            WindowMinimumHeight = 400;
            WindowMinimumWidth = 700;

            Track = track;

            AddToPlaylistCommand = new RelayParameterizedCommand(AddToPlaylist);
            LoadPlaylistGroups();
        }

        #region Commands

        public ICommand AddPlaylistGroupCommand { get; set; }

        public ICommand AddToPlaylistCommand { get; set; }

        #endregion

        #region Properties

        public ObservableCollection<PlaylistGroupItem> PlaylistGroupItems { get; set; } =
            new ObservableCollection<PlaylistGroupItem>();

        public Models.Track.Track Track { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Load playlist groups and related playlists
        /// </summary>
        public void LoadPlaylistGroups()
        {
            using var db = new AppDbContext();

            PlaylistGroupItems = db.PlaylistGroups.Select(x => new PlaylistGroupItem
            {
                PlaylistGroup = x,
                Playlists = db.Playlists.Where(c => c.PlaylistGroupId == x.Id).ToObservableCollection(),
                AddPlayListCommand = new RelayParameterizedCommand(AddPlayList),
                PlaylistGroupItemExpandedCommand = new RelayParameterizedCommand(PlaylistGroupItemExpanded)
            }).ToObservableCollection();
        }

        #endregion

        #region Command Methods

        public void AddToPlaylist(object sender)
        {
            var playlist = ((Button)sender).DataContext as Models.Playlist.Playlist;

            using var db = new AppDbContext();

            if (!db.PlaylistTracks.Any(x => x.PlaylistId == playlist.Id && x.TrackId == Track.Id)){
                db.PlaylistTracks.Add(new Models.Playlist.PlaylistTrack 
                { 
                    TrackId = Track.Id,
                    PlaylistId = playlist.Id
                });
                db.SaveChanges();

                mWindow.Close();
            }
        }

        public void AddPlayList(object sender)
        {
            var playlistGroupItem = (sender as Button).DataContext as PlaylistGroupItem;

            var dialog = new AddPlaylistDialog();

            dialog.Closing += (sender, args) =>
            {
                if (dialog.DataContext is AddPlaylistViewModel vm)
                {
                    if (!playlistGroupItem.Playlists.Any(x => x == vm.Playlist) && vm.Playlist.Id > 0)
                    {
                        playlistGroupItem.Playlists.Insert(0, vm.Playlist);

                        foreach (var item in PlaylistGroupItems)
                        {
                            item.IsExpanded = false;
                        }

                        playlistGroupItem.IsExpanded = true;
                    }
                }
            };

            dialog.ShowDialogWindow(new AddPlaylistViewModel(dialog, playlistGroupItem.PlaylistGroup), mWindow);
        }

        public void PlaylistGroupItemExpanded(object sender)
        {
            var values = (object[])sender;
            var listview = values[0] as ListView;
            var currentExpander = values[1] as Expander;


            foreach (var item in listview.Items)
            {
                if (item is PlaylistGroupItem playlistGroupItem)
                {
                    _ = playlistGroupItem == currentExpander.DataContext as PlaylistGroupItem ?
                        playlistGroupItem.IsExpanded = true :
                        playlistGroupItem.IsExpanded = false;
                }
            }
        }

        #endregion
    }
}
