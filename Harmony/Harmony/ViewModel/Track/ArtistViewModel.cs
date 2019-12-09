using GalaSoft.MvvmLight;
using Harmony.Data;
using Harmony.Helpers;
using Harmony.Models.Common;
using Harmony.Models.Track;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using static Harmony.DI.DI;

namespace Harmony.ViewModel.Track
{
    public class ArtistViewModel : ViewModelBase
    {
        public ArtistViewModel()
        {
            GoToAlbumCommand = new RelayParameterizedCommand(GoToAlbum);

        }

        #region Commands

        public ICommand GoToAlbumCommand { get; set; }

        #endregion

        #region Properties


        #endregion

        #region Methods


        #endregion

        #region Command Methods

        public void GoToAlbum(object sender)
        {
            using var db = new AppDbContext();

            var albumItem = (sender as Button).DataContext as AlbumItem;

            albumItem.TrackItems = db.Tracks.Where(track => track.AlbumId == albumItem.Album.Id)
            .Select(track => new TrackItem
            {
                Track = track,
            }).ToObservableCollection();

            for (int i = 0; i < albumItem.TrackItems.Count; i++)
            {
                albumItem.TrackItems[i].Order = i + 1;
            }

            foreach (var artist in db.AlbumArtists.Where(c => c.AlbumId == albumItem.Album.Id))
            {
                albumItem.Artists.Add(db.Artists.FirstOrDefault(x => x.Id == artist.ArtistId));
            }

            if (ViewModelApplication.CurrentPage != ApplicationPage.Album)
            {
                ViewModelApplication.SelectedAlbumItem = albumItem;
                ViewModelApplication.GoToPage(ApplicationPage.Album);
            }
        }

        #endregion
    }
}