using Harmony.Data;
using Harmony.Helpers;
using Harmony.Models.Playlist;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Harmony.ViewModel.Playlist
{
    public class AddPlaylistViewModel : WindowViewModel
    {
        public AddPlaylistViewModel(Window window, PlaylistGroup playlistGroup = null, Models.Playlist.Playlist playlist = null) : base(window)
        {
            mWindow = window;
            WindowMinimumHeight = 400;
            WindowMinimumWidth = 700;

            Playlist = playlist ?? new Models.Playlist.Playlist();
            PlaylistGroup = playlistGroup ?? new PlaylistGroup();

            Title = playlist != null ?
                $"Update Playlist: {playlist.Title}" :
                "Add New Playlist";

            CloseCommand = new RelayCommand(p =>
            {
                AddOrUpdate();

                mWindow.Close();
            });

            AddImageCommand = new RelayCommand(p => AddImage());
        }

        #region Commands

        public ICommand AddImageCommand { get; set; }

        #endregion

        #region Prperties

        public Models.Playlist.Playlist Playlist { get; set; }
        public PlaylistGroup PlaylistGroup { get; set; }

        #endregion

        #region Methods

        public void AddOrUpdate()
        {
            if (!string.IsNullOrEmpty(Playlist.Title) && PlaylistGroup.Id > 0)
            {
                using var db = new AppDbContext();

                Playlist.PlaylistGroupId = PlaylistGroup.Id;

                _ = PlaylistGroup.Id > 0 ?
                    db.Playlists.Update(Playlist) :
                    db.Playlists.Add(Playlist); ;

                db.SaveChanges();
            }

        }

        #endregion

        #region Command Methods

        /// <summary>
        /// Add playlist image
        /// </summary>
        public void AddImage()
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Select Playlist Image",
                Filter = Settings.ImageFilter,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (openFileDialog.ShowDialog() == true)
            {
                Playlist.Image = openFileDialog.FileName.PathToBitmapImage();
            }
        }

        #endregion
    }
}
