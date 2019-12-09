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
    public class AlbumViewModel : ViewModelBase
    {
        public AlbumViewModel()
        {
            GoToArtistCommand = new RelayParameterizedCommand(GoToArtist);
            PlayAsPlaylistCommand = new RelayCommand(p => PlayAsPlaylist());
        }

        #region Commands

        public ICommand PlayAsPlaylistCommand { get; set; }
        public ICommand GoToArtistCommand { get; set; }

        #endregion

        #region Properties


        #endregion

        #region Methods


        #endregion

        #region Command Methods

        public void PlayAsPlaylist()
        {
            if(ViewModelApplication.SelectedAlbumItem != null && ViewModelApplication.SelectedAlbumItem.TrackItems.Count() > 0)
            {
                ViewModelApplication.PlaylistTrackItems = ViewModelApplication.SelectedAlbumItem.TrackItems;
                ViewModelApplication.PlayTrackItem(ViewModelApplication.GetNextTrackItem(ViewModelApplication.PlaylistTrackItems, null));
            }
        }

        public void GoToArtist(object sender)
        {
            using var db = new AppDbContext();

            var artist = (sender as Button).DataContext as Artist;
            var artistItem = new ArtistItem 
            { 
                Artist = artist
            };

            // get contributed albums
            var albumArtists = db.AlbumArtists.Where(x => x.ArtistId == artist.Id).ToObservableCollection();

            // get albums
            foreach (var albumArtist in albumArtists)
            {
                var albumItem = db.Albums.Select(album => new AlbumItem
                {
                    Album = new Album
                    {
                        Id = album.Id,
                        Title = album.Title,
                        ArtistId = album.ArtistId,
                        Description = album.Description,
                        ReleaseDate = album.ReleaseDate,
                        Image = album.Image ?? db.Tracks.FirstOrDefault(x => x.AlbumId == x.Id && x.Image != null).Image
                    }
                }).FirstOrDefault(x => x.Album.Id == albumArtist.AlbumId);

                if (albumItem != null)
                {
                    artistItem.AlbumItems.Add(albumItem);
                }
            }

            if (ViewModelApplication.CurrentPage != ApplicationPage.Artist)
            {
                ViewModelApplication.SelectedArtistItem = artistItem;
                ViewModelApplication.GoToPage(ApplicationPage.Artist);
            }
        }

        #endregion
    }
}