using GalaSoft.MvvmLight;
using Harmony.Data;
using Harmony.Helpers;
using Harmony.Models.Common;
using Harmony.Models.Track;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using static Harmony.DI.DI;

namespace Harmony.ViewModel.Track
{
    public class AlbumsViewModel : ViewModelBase
    {
        public AlbumsViewModel()
        {
            GoToAlbumCommand = new RelayParameterizedCommand(GoToAlbum);
            UnloadedCommand = new RelayCommand(p => Unloaded());
            LoadAlbums();
        }

        #region Commands

        public ICommand GoToAlbumCommand { get; set; }
        public ICommand UnloadedCommand { get; set; }

        #endregion

        #region Properties

        public ObservableCollection<AlbumItem> AlbumItems { get; set; } = new ObservableCollection<AlbumItem>();

        #endregion

        #region Methods
        public void Unloaded()
        {
            AlbumItems = new ObservableCollection<AlbumItem>();
        }
        public void LoadAlbums()
        {
            using var db = new AppDbContext();

            AlbumItems = db.Albums.Select(album => new AlbumItem
            { 
                Album = album,
            }).OrderBy(x => x.Album.Title).ToObservableCollection();

            foreach (var albumItem in AlbumItems)
            {
                if (albumItem.Album.Image == null)
                {
                    albumItem.Album.Image = db.Tracks.FirstOrDefault(x => x.AlbumId == albumItem.Album.Id && x.Image != null)?.Image;
                }
            }

            ViewModelApplication.SelectedAlbumItem = AlbumItems.FirstOrDefault();
        }

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