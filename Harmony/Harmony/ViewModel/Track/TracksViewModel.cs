using GalaSoft.MvvmLight;
using Harmony.Data;
using Harmony.Helpers;
using Harmony.Models.Track;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace Harmony.ViewModel.Track
{
    public class TracksViewModel : ViewModelBase
    {
        public TracksViewModel()
        {
            GoToListPageCommand = new RelayParameterizedCommand(GoToListPage);
            SearchChangedCommand = new RelayParameterizedCommand(SearchChanged);
            PaginationChangedCommand = new RelayCommand(p => LoadTrackItems());
            LoadTrackItems();
        }

        #region Commands

        public ICommand PaginationChangedCommand { get; set; }
        public ICommand SearchChangedCommand { get; set; }
        public ICommand GoToListPageCommand { get; set; }


        #endregion

        #region Properties

        public ObservableCollection<TrackItem> TrackItems { get; set; } = new ObservableCollection<TrackItem>();
        public int PageLimit { get; set; } = 100;
        public int CurrentPage { get; set; } = 1;
        public string SearchTerm { get; set; }
        public Pagination Pagination { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Load tracks
        /// </summary>
        public void LoadTrackItems()
        {
            using var db = new AppDbContext();

            var totalSize = db.Tracks.Where(x => EF.Functions.Like(x.Title, $"%{SearchTerm}%")).Count();
            totalSize = totalSize > 0 ? totalSize : 1;

            Pagination = new Pagination(totalSize, CurrentPage, PageLimit, 10);

            TrackItems = db.Tracks.Where(x => EF.Functions.Like(x.Title, $"%{SearchTerm}%"))
            .Select(track => new TrackItem
            {
                Track = track,
            })
            .OrderBy(x => x.Track.Title)
            .Skip((CurrentPage - 1) * PageLimit)
            .Take(PageLimit)
            .ToObservableCollection();
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// Go to page
        /// </summary>
        /// <param name="sender"></param>
        public void GoToListPage(object sender)
        {
            CurrentPage = (int)(sender as Button).DataContext;
            LoadTrackItems();
        }

        /// <summary>
        /// Search term changed
        /// </summary>
        /// <param name="sender"></param>
        public void SearchChanged(object sender)
        {
            SearchTerm = (sender as TextBox).Text;
            CurrentPage = 1;
            LoadTrackItems();
        }

        #endregion
    }
}