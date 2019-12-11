using GalaSoft.MvvmLight;
using Harmony.Data;
using Harmony.Helpers;
using Harmony.Models.Common;
using Harmony.Models.Playlist;
using Harmony.Models.Track;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using static Harmony.DI.DI;

namespace Harmony.ViewModel.Playlist
{
    public class DailyMixesViewModel : ViewModelBase
    {
        public DailyMixesViewModel()
        {
            GoToDailyMixCommand = new RelayParameterizedCommand(GoToDailyMix);
            GoToListPageCommand = new RelayParameterizedCommand(GoToListPage);
            SearchChangedCommand = new RelayParameterizedCommand(SearchChanged);
            PaginationChangedCommand = new RelayCommand(p => LoadDailyMixes());

            LoadDailyMixes();
        }

        #region Commands

        public ICommand GoToDailyMixCommand { get; set; }
        public ICommand PaginationChangedCommand { get; set; }
        public ICommand SearchChangedCommand { get; set; }
        public ICommand GoToListPageCommand { get; set; }

        #endregion

        #region Properties

        public ObservableCollection<DailyMix> DailyMixes { get; set; } = new ObservableCollection<DailyMix>();
        public int PageLimit { get; set; } = 100;
        public int CurrentPage { get; set; } = 1;
        public string SearchTerm { get; set; }
        public Pagination Pagination { get; set; }

        #endregion

        #region Methods

        public void LoadDailyMixes()
        {
            using var db = new AppDbContext();

            DailyMixes = db.DailyMixes.ToObservableCollection();

            var totalSize = db.DailyMixes.Count();
            totalSize = totalSize > 0 ? totalSize : 1;

            Pagination = new Pagination(totalSize, CurrentPage, PageLimit, 10);

            DailyMixes = db.DailyMixes
            .Skip((CurrentPage - 1) * PageLimit)
            .Take(PageLimit)
            .ToObservableCollection();
        }

        #endregion

        #region Command Methods

        public void GoToListPage(object sender)
        {
            CurrentPage = (int)(sender as Button).DataContext;
            LoadDailyMixes();
        }

        public void SearchChanged(object sender)
        {
            SearchTerm = (sender as TextBox).Text;
            CurrentPage = 1;
            LoadDailyMixes();
        }

        public void GoToDailyMix(object sender)
        {
            var dailyMix = (sender as Button).DataContext as DailyMix;
            using var db = new AppDbContext();

            if (ViewModelApplication.CurrentPage != ApplicationPage.DailyMix)
            {

                var dailyMixTracks = db.DailyMixTracks.Where(x => x.DailyMixId == dailyMix.Id).ToObservableCollection();

                var dailyMixTrackItems = new ObservableCollection<TrackItem>();

                foreach (var dailyMixTrack in dailyMixTracks)
                {
                    dailyMixTrackItems.Add(new TrackItem 
                    {
                        Track = db.Tracks.First(x => x.Id == dailyMixTrack.TrackId)
                    });
                }

                ViewModelApplication.SelectedDailyMix = dailyMix;
                ViewModelApplication.SelectedPlaylistTrackItems = dailyMixTrackItems;
                ViewModelApplication.GoToPage(ApplicationPage.DailyMix);
            }
        }

        #endregion
    }
}