using GalaSoft.MvvmLight;
using Harmony.Data;
using Harmony.Helpers;
using Harmony.Models.Common;
using Harmony.Models.Track;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using static Harmony.DI.DI;

namespace Harmony.ViewModel.Track
{
    public class ArtistsViewModel : ViewModelBase
    {
        public ArtistsViewModel()
        {
            GoToListPageCommand = new RelayParameterizedCommand(GoToListPage);
            GoToArtistCommand = new RelayParameterizedCommand(GoToArtist);
            SearchChangedCommand = new RelayParameterizedCommand(SearchChanged);
            PaginationChangedCommand = new RelayCommand(p => LoadArtists());
            LoadArtists();
        }

        #region Commands

        public ICommand GoToArtistCommand { get; set; }
        public ICommand PaginationChangedCommand { get; set; }
        public ICommand SearchChangedCommand { get; set; }

        public ICommand GoToListPageCommand { get; set; }


        #endregion

        #region Properties

        public ObservableCollection<ArtistItem> ArtistItems { get; set; } = new ObservableCollection<ArtistItem>();
        public int PageLimit { get; set; } = 100;
        public int CurrentPage { get; set; } = 1;
        public string SearchTerm { get; set; }
        public Pagination Pagination { get; set; }
        #endregion

        #region Methods

        public void LoadArtists()
        {
            using var db = new AppDbContext();

            var totalSize = db.Artists.Where(x => EF.Functions.Like(x.FullName, $"%{SearchTerm}%")).Count();
            totalSize = totalSize > 0 ? totalSize : 1;

            Pagination = new Pagination(totalSize, CurrentPage, PageLimit, 10);

            ArtistItems = db.Artists.Where(x => EF.Functions.Like(x.FullName, $"%{SearchTerm}%"))
            .Select(artist => new ArtistItem 
            {
                Artist = artist,
            })
            .Skip((CurrentPage - 1) * PageLimit)
            .Take(PageLimit)
            .ToObservableCollection();
        }

        #endregion

        #region Command Methods

        public void GoToListPage(object sender)
        {
            CurrentPage = (int)(sender as Button).DataContext;
            LoadArtists();
        }

        public void GoToArtist(object sender)
        {
            using var db = new AppDbContext();

            var artistItem = (sender as Button).DataContext as ArtistItem;

            // get contributed albums
            var albumArtists = db.AlbumArtists.Where(x => x.ArtistId == artistItem.Artist.Id).ToObservableCollection();

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
                        ReleaseDate = album.ReleaseDate
                    }
                }).FirstOrDefault(x => x.Album.Id == albumArtist.AlbumId);

                if (albumItem != null){
                    if (albumItem.Album.Image == null)
                    {
                        albumItem.Album.Image = db.Tracks.FirstOrDefault(x => x.AlbumId == albumItem.Album.Id && x.Image != null)?.Image;
                    }

                    artistItem.AlbumItems.Add(albumItem);
                }
            }

            if (ViewModelApplication.CurrentPage != ApplicationPage.Artist)
            {
                ViewModelApplication.SelectedArtistItem = artistItem;
                ViewModelApplication.GoToPage(ApplicationPage.Artist);
            }
        }

        public void SearchChanged(object sender)
        {
            SearchTerm = (sender as TextBox).Text;
            CurrentPage = 1;
            LoadArtists();
        }

        #endregion
    }
}
