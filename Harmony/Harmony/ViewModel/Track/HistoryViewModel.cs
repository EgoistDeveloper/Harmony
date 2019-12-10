using GalaSoft.MvvmLight;
using Harmony.Data;
using Harmony.Helpers;
using Harmony.Models.Track;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace Harmony.ViewModel.Track
{
    public class HistoryViewModel : ViewModelBase
    {
        public HistoryViewModel()
        {
            GoToListPageCommand = new RelayParameterizedCommand(GoToListPage);
            SearchChangedCommand = new RelayParameterizedCommand(SearchChanged);
            PaginationChangedCommand = new RelayCommand(p => LoadPlayHistory());

            LoadPlayHistory();
        }

        #region Commands

        public ICommand GoToArtistCommand { get; set; }
        public ICommand PaginationChangedCommand { get; set; }
        public ICommand SearchChangedCommand { get; set; }

        public ICommand GoToListPageCommand { get; set; }

        #endregion

        #region Properties

        public ObservableCollection<PlaysItem> PlaysItems { get; set; } = new ObservableCollection<PlaysItem>();
        public int PageLimit { get; set; } = 100;
        public int CurrentPage { get; set; } = 1;
        public string SearchTerm { get; set; }
        public Pagination Pagination { get; set; }
        #endregion

        #region Methods

        public void LoadPlayHistory()
        {
            using var db = new AppDbContext();

            var totalSize = db.Plays.Count();
            totalSize = totalSize > 0 ? totalSize : 1;

            Pagination = new Pagination(totalSize, CurrentPage, PageLimit, 10);

            PlaysItems = db.Plays
            .Select(plays => new PlaysItem
            {
                Plays = plays,
                Track = db.Tracks.FirstOrDefault(x => x.Id == plays.TrackId)
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
            LoadPlayHistory();
        }

        public void SearchChanged(object sender)
        {
            SearchTerm = (sender as TextBox).Text;
            CurrentPage = 1;
            LoadPlayHistory();
        }

        #endregion
    }
}
