﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Shapes;
using GalaSoft.MvvmLight;
using Harmony.Data;
using Harmony.Dialogs.Playlist;
using Harmony.Helpers;
using Harmony.Models.Common;
using Harmony.Models.Playlist;
using Harmony.ViewModel.Playlist;
using static Harmony.DI.DI;

namespace Harmony.ViewModel.App
{
    public class NavbarViewModel : ViewModelBase
    {
        public NavbarViewModel()
        {
            // default navbar items
            NavbarItems = new List<NavbarItem>()
            {
                new NavbarItem()
                {
                    ApplicationPage = ApplicationPage.DialyMix,
                    IconData = (System.Windows.Application.Current.FindResource("CurrentAc") as Path)?.Data
                },
                new NavbarItem()
                {
                    ApplicationPage = ApplicationPage.History,
                    IconData = (System.Windows.Application.Current.FindResource("History") as Path)?.Data
                },
                new NavbarItem()
                {
                    ApplicationPage = ApplicationPage.Tracks,
                    IconData = (System.Windows.Application.Current.FindResource("MusicNote") as Path)?.Data
                },
                new NavbarItem()
                {
                    ApplicationPage = ApplicationPage.Albums,
                    IconData = (System.Windows.Application.Current.FindResource("Album") as Path)?.Data
                },
                new NavbarItem()
                {
                    ApplicationPage = ApplicationPage.Artists,
                    IconData = (System.Windows.Application.Current.FindResource("Artist") as Path)?.Data
                },
                new NavbarItem()
                {
                    ApplicationPage = ApplicationPage.Podcasts,
                    IconData = (System.Windows.Application.Current.FindResource("Podcast") as Path)?.Data
                }
            };

            GoToCommand = new RelayParameterizedCommand(GoTo);
            AddPlaylistGroupCommand = new RelayCommand(p => AddPlaylistGroup());

            LoadPlaylistGroups();
        }

        #region Commands

        public ICommand AddPlaylistGroupCommand { get; set; }

        public ICommand GoToCommand { get; set; }

        #endregion

        #region Properties

        public ObservableCollection<PlaylistGroupItem> PlaylistGroupItems { get; set; } = 
            new ObservableCollection<PlaylistGroupItem>();

        public List<NavbarItem> NavbarItems { get; set; }

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

        /// <summary>
        /// Go to selected page
        /// </summary>
        /// <param name="sender"></param>
        public void GoTo(object sender)
        {
            var navbarItem = (sender as Button).DataContext as NavbarItem;

            if (ViewModelApplication.CurrentPage != navbarItem.ApplicationPage)
            {
                ViewModelApplication.GoToPage(navbarItem.ApplicationPage);
            }
        }

        /// <summary>
        /// Add play list group
        /// </summary>
        public void AddPlaylistGroup()
        {
            var dialog = new AddPlaylistGroupDialog();

            dialog.Closing += (sender, args) =>
            {
                if (dialog.DataContext is AddPlaylistGroupViewModel vm)
                {
                    if (!PlaylistGroupItems.Any(x => x.PlaylistGroup == vm.PlaylistGroup) && vm.PlaylistGroup.Id > 0)
                    {
                        PlaylistGroupItems.Insert(0, new PlaylistGroupItem 
                        { 
                            PlaylistGroup = vm.PlaylistGroup,
                            AddPlayListCommand = new RelayParameterizedCommand(AddPlayList),
                            IsExpanded = true
                        });
                    }
                }
            };

            dialog.ShowDialogWindow(new AddPlaylistGroupViewModel(dialog));
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

            dialog.ShowDialogWindow(new AddPlaylistViewModel(dialog, playlistGroupItem.PlaylistGroup));
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