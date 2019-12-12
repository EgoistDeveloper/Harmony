using Harmony.Data;
using Harmony.Models.Playlist;
using System.Windows;

namespace Harmony.ViewModel.Playlist
{
    public class AddPlaylistGroupViewModel : WindowViewModel
    {
        public AddPlaylistGroupViewModel(Window window, PlaylistGroup playlistGroup = null) : base(window)
        {
            mWindow = window;
            WindowMinimumHeight = 350;
            WindowMinimumWidth = 500;

            PlaylistGroup = playlistGroup ?? new PlaylistGroup();

            Title = playlistGroup != null ?
                $"Update Playlist Group: {playlistGroup.Title}" :
                "Add New Playlist Group";

            CloseCommand = new RelayCommand(p =>
            {
                AddOrUpdate();

                mWindow.Close();
            });
        }

        #region Prperties

        public PlaylistGroup PlaylistGroup { get; set; }

        #endregion

        #region Methods
        
        public void AddOrUpdate()
        {
            if (!string.IsNullOrEmpty(PlaylistGroup.Title))
            {
                using var db = new AppDbContext();

                _ = PlaylistGroup.Id > 0 ?
                db.PlaylistGroups.Update(PlaylistGroup) :
                db.PlaylistGroups.Add(PlaylistGroup);

                db.SaveChanges();

            }
        }

        #endregion
    }
}