using GalaSoft.MvvmLight;
using Harmony.Data;
using Harmony.Helpers;
using Harmony.Models.Common;
using Harmony.Models.Player;
using Harmony.Models.Player.Enums;
using Harmony.Models.Track;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using static Harmony.DI.DI;


namespace Harmony.ViewModel.App
{
    /// <summary>
    /// The application state as a view model
    /// </summary>
    public class ApplicationViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public ApplicationViewModel()
        {
            using var db = new AppDbContext();

            AppSettings = new AppSettings();

            #region NAudip Player Setup

            NAudioPlayer = new NAudioPlayer();

            NAudioPlayer.PlaybackPaused += _audioPlayer_PlaybackPaused;
            NAudioPlayer.PlaybackResumed += NAudioPlayer_PlaybackResumed;
            NAudioPlayer.PlaybackStopped += NAudioPlayer_PlaybackStopped;

            PlayerTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            PlayerTimer.Tick += PlayerTick;

            #endregion


            CurrentVolume = 0.7f; // todo: get settings from db

            #region App Commands

            GoToImporterCommand = new RelayCommand(p => GoToImporter());

            #endregion

            // Menu commands
            AddFileToPlaylistCommand = new RelayCommand(AddFileToPlaylist, CanAddFileToPlaylist);
            AddFolderToPlaylistCommand = new RelayCommand(AddFolderToPlaylist, CanAddFolderToPlaylist);
            SavePlaylistCommand = new RelayCommand(SavePlaylist, CanSavePlaylist);
            LoadPlaylistCommand = new RelayCommand(LoadPlaylist, CanLoadPlaylist);

            #region Player commands

            StartPlaybackCommand = new RelayCommand(StartPlayback, CanStartPlayback);
            PlayTrackItemCommand = new RelayParameterizedCommand(PlayTrackItem);
            RewindToStartCommand = new RelayCommand(p => RewindToStart());
            ForwardToEndCommand = new RelayCommand(p => ForwardToEnd());

            VolumeChangedCommand = new RelayCommand(p => VolumeChanged());
            PositionChangedCommand = new RelayParameterizedCommand(PositionChanged);

            #endregion

            ShuffleCommand = new RelayCommand(Shuffle, CanShuffle);
        }

        #region Commands

        #region App Commands

        public ICommand GoToImporterCommand { get; set; }
        public ICommand PlayTrackItemCommand { get; set; }
        #endregion

        #region Player Commands

        public ICommand StartPlaybackCommand { get; set; }
        public ICommand RewindToStartCommand { get; set; }
        public ICommand ForwardToEndCommand { get; set; }

        public ICommand VolumeChangedCommand { get; set; }
        public ICommand PositionChangedCommand { get; set; }

        public ICommand AddToPlaylistCommand { get; set; }

        #endregion

        #endregion

        #region Properties

        #region Host

        public ApplicationPage CurrentPage { get; set; }

        public ViewModelBase CurrentPageViewModel { get; set; }

        public AppSettings AppSettings { get; set; }

        #endregion

        #region AudioPlayer

        public NAudioPlayer NAudioPlayer { get; set; }

        public DispatcherTimer PlayerTimer { get; set; }

        #region Duration - Length - Remaining

        public TimeSpan AudioLength { get; set; }
        public double AudioLengthInSeconds { get; set; }

        public TimeSpan AudioPosition { get; set; }
        public double AudioPositionInSeconds { get; set; }

        public TimeSpan AudioRemaining { get; set; }

        public double CurrentTrackLength { get; set; }

        public RepeatType Repeat { get; set; } = RepeatType.Repeat; // todo: load from db

        public TimeSpan TrackPlayTime { get; set; }
        #endregion

        #region Volume

        public float CurrentVolume { get; set; }

        public float MaximumVolume { get; set; } = 1f;

        #endregion

        #endregion

        #region Track - Playlist

        public Models.Track.Track Track { get; set; }

        public Models.Playlist.PlaylistItem PlaylistItem { get; set; }

        #endregion

        #region Others

        public ObservableCollection<AlbumItem> AlbumItems { get; set; } = new ObservableCollection<AlbumItem>();
        public AlbumItem SelectedAlbumItem { get; set; }

        public TrackItem CurrentlySelectedTrack { get; set; }
        public ObservableCollection<TrackItem> PlaylistTrackItems { get; set; } = new ObservableCollection<TrackItem>();

        public ObservableCollection<TrackItem> ArtistTrackItems { get; set; } = new ObservableCollection<TrackItem>();
        public ArtistItem SelectedArtistItem { get; set; }
        #endregion

        #endregion

        #region Methods

        #region GoToPage

        /// <summary>
        /// Navigates to the specified page
        /// </summary>
        /// <param name="page">The page to go to</param>
        /// <param name="viewModel">The view model, if any, to set explicitly to the new page</param>
        public void GoToPage(ApplicationPage page, ViewModelBase viewModel = null)
        {
            CurrentPageViewModel = viewModel;

            var different = CurrentPage != page;

            CurrentPage = page;

            if (!different)
                OnPropertyChanged(nameof(CurrentPage));
        }

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        public void OnPropertyChanged(string name)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        #endregion

        #region Command Methods

        #region App Command Methods

        /// <summary>
        /// Go to importer page
        /// </summary>
        public void GoToImporter()
        {
            if (ViewModelApplication.CurrentPage != ApplicationPage.Importer)
            {
                ViewModelApplication.GoToPage(ApplicationPage.Importer);
            }
        }

        #endregion

        #region Player Methods

        /// <summary>
        /// Start playback
        /// </summary>
        /// <param name="sender"></param>
        public void StartPlayback(object sender)
        {
            if (CurrentlySelectedTrack != null)
            {
                if (NAudioPlayer.WaveOutEvent.PlaybackState == NAudio.Wave.PlaybackState.Stopped)
                {
                    PlayerTimer.Stop();
                }

                if (CurrentlySelectedTrack != null)
                {
                    CurrentTrackLength = NAudioPlayer.GetLengthInSeconds();
                    AudioLength = NAudioPlayer.GetLength();
                    AudioLengthInSeconds = NAudioPlayer.GetLengthInSeconds();

                    PlayerTimer.Start();
                    NAudioPlayer.TogglePlayPause(CurrentVolume);
                }
            }
        }

        /// <summary>
        /// Can start playback
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool CanStartPlayback(object p)
        {
            return CurrentlySelectedTrack != null;
        }

        /// <summary>
        /// Rewind to start or play previous track
        /// </summary>
        public void RewindToStart()
        {
            if (NAudioPlayer.GetPositionInSeconds() < 5)
            {
                if (PlaylistTrackItems != null && PlaylistTrackItems.Count() > 0)
                {
                    PlayTrackItem(GetPreviousTrackItem(PlaylistTrackItems, CurrentlySelectedTrack));
                }
            }
            else
            {
                NAudioPlayer.Rewind();
            }
        }

        /// <summary>
        /// Forward to end or play next track
        /// </summary>
        public void ForwardToEnd()
        {
            if (NAudioPlayer.GetPositionInSeconds() < NAudioPlayer.GetLengthInSeconds() - 5)
            {
                if (PlaylistTrackItems != null && PlaylistTrackItems.Count() > 0)
                {
                    PlayTrackItem(GetNextTrackItem(PlaylistTrackItems, CurrentlySelectedTrack));
                }
            }
            else
            {
                NAudioPlayer.Forward();
            }
        }

        /// <summary>
        /// Play TrackItem
        /// </summary>
        /// <param name="sender">TrackItem</param>
        public void PlayTrackItem(object sender)
        {
            if (sender != null)
            {
                var trackItem = new TrackItem();

                // get directly (playlist next)
                if (sender is TrackItem _trackItem)
                {
                    trackItem = _trackItem;
                }
                // get in list
                else
                {
                    trackItem = (sender as ToggleButton).DataContext as TrackItem;
                }

                // if there is track
                if (trackItem.Track != null)
                {
                    if (CurrentlySelectedTrack == trackItem)
                    {
                        NAudioPlayer.TogglePlayPause(CurrentVolume);
                    }
                    else
                    {
                        // if there is selected track set is playing false
                        if (CurrentlySelectedTrack != null)
                        {
                            CurrentlySelectedTrack.IsPlaying = false;
                            CurrentlySelectedTrack = null;
                        }

                        // dispose previous player/track
                        NAudioPlayer.Dispose();

                        // set new track and play
                        CurrentlySelectedTrack = trackItem;
                        NAudioPlayer.Play(NAudio.Wave.PlaybackState.Stopped, CurrentVolume, trackItem.Track.FileLocation);
                        trackItem.IsPlaying = true;

                        // get lengths
                        CurrentTrackLength = NAudioPlayer.GetLengthInSeconds();
                        AudioLength = NAudioPlayer.GetLength();
                        AudioLengthInSeconds = NAudioPlayer.GetLengthInSeconds();

                        // get setprogress and remaining etc
                        PlayerTimer.Start();
                    }
                }
            }
        }

        /// <summary>
        /// Set new position
        /// </summary>
        /// <param name="sender"></param>
        public void PositionChanged(object sender)
        {
            var slider = sender as Slider;

            NAudioPlayer.SetPosition(slider.Value);
        }

        /// <summary>
        /// Set volume
        /// </summary>
        public void VolumeChanged()
        {
            if (CurrentlySelectedTrack != null)
            {
                NAudioPlayer.SetVolume(CurrentVolume);
            }
        }

        #endregion

        #endregion


        public ICommand AddFileToPlaylistCommand { get; set; }
        public ICommand AddFolderToPlaylistCommand { get; set; }
        public ICommand SavePlaylistCommand { get; set; }
        public ICommand LoadPlaylistCommand { get; set; }
        public ICommand ShuffleCommand { get; set; }

        private void AddFileToPlaylist(object p)
        {

        }
        private bool CanAddFileToPlaylist(object p)
        {
            return true;
        }

        private void AddFolderToPlaylist(object p)
        {

        }

        private bool CanAddFolderToPlaylist(object p)
        {
            return true;
        }

        private void SavePlaylist(object p)
        {

        }

        private bool CanSavePlaylist(object p)
        {
            return true;
        }

        private void LoadPlaylist(object p)
        {

        }

        private bool CanLoadPlaylist(object p)
        {
            return true;
        }








        private void Shuffle(object p)
        {

        }
        private bool CanShuffle(object p)
        {
            return true;
        }

        #region Event Methods

        /// <summary>
        /// Player timer for current position and remaing
        /// </summary>
        /// <param name="sernder"></param>
        /// <param name="e"></param>
        public void PlayerTick(object sernder, EventArgs e)
        {
            AudioPosition = NAudioPlayer.GetPosition();
            AudioPositionInSeconds = NAudioPlayer.GetPositionInSeconds();
            AudioRemaining = NAudioPlayer.GetRemaining();
        }

        /// <summary>
        /// NAudioPlayer stopped
        /// </summary>
        private void NAudioPlayer_PlaybackStopped()
        {
            // if playlist is not null play next
            if (PlaylistTrackItems != null && PlaylistTrackItems.Count() > 0)
            {
                PlayTrackItem(GetNextTrackItem(PlaylistTrackItems, CurrentlySelectedTrack));
            }
        }

        /// <summary>
        /// NAudioPlayer resumed
        /// </summary>
        private void NAudioPlayer_PlaybackResumed()
        {
            
        }

        /// <summary>
        /// NAudioPlayer paused
        /// </summary>
        private void NAudioPlayer_PlaybackPaused()
        {
            
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Get next TrackItem in TrackItems
        /// </summary>
        /// <typeparam name="TrackItem"></typeparam>
        /// <param name="trackItems"></param>
        /// <param name="currentItem"></param>
        /// <returns></returns>
        public TrackItem GetNextTrackItem(ObservableCollection<TrackItem> trackItems, TrackItem currentItem = null)
        {
            if (currentItem != null)
            {
                // get current item index
                var currentIndex = trackItems.IndexOf(currentItem);

                // if is not last one
                if (currentIndex < trackItems.Count - 1)
                {
                    return trackItems[currentIndex + 1];
                }
            }

            // return first
            return trackItems[0];
        }

        /// <summary>
        /// Get previous TrackItem in TrackItems
        /// </summary>
        /// <typeparam name="TrackItem"></typeparam>
        /// <param name="trackItems"></param>
        /// <param name="currentItem"></param>
        /// <returns></returns>
        public TrackItem GetPreviousTrackItem(ObservableCollection<TrackItem> trackItems, TrackItem currentItem = null)
        {
            if (currentItem != null)
            {
                // get current item index
                var currentIndex = trackItems.IndexOf(currentItem);

                // if is not first one
                if (currentIndex - 1 > 0)
                {
                    return trackItems[currentIndex - 1];
                }
            }

            // return last
            return trackItems[trackItems.Count - 1];
        }

        #endregion
    }
}