using GalaSoft.MvvmLight;
using Harmony.Data;
using Harmony.Dialogs.Playlist;
using Harmony.Helpers;
using Harmony.Models.Common;
using Harmony.Models.Player;
using Harmony.Models.Player.Enums;
using Harmony.Models.Track;
using Harmony.ViewModel.Playlist;
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

            NAudioPlayer.PlaybackPaused += NAudioPlayer_PlaybackPaused;
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


            #region Player commands

            StartPlaybackCommand = new RelayCommand(StartPlayback, CanStartPlayback);
            PlayTrackItemCommand = new RelayParameterizedCommand(PlayTrackItem);
            RewindToStartCommand = new RelayCommand(p => RewindToStart());
            ForwardToEndCommand = new RelayCommand(p => ForwardToEnd());
            ChangeRepeatTypeCommand = new RelayCommand(p => ChangeRepeatType());

            VolumeChangedCommand = new RelayCommand(p => VolumeChanged());
            PositionChangedCommand = new RelayParameterizedCommand(PositionChanged);
            ValueChangedCommand = new RelayParameterizedCommand(ValueChanged);

            AddToPlaylistCommand = new RelayCommand(p => AddToPlaylist());
            #endregion
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
        public ICommand ChangeRepeatTypeCommand { get; set; }

        public ICommand VolumeChangedCommand { get; set; }
        public ICommand PositionChangedCommand { get; set; }
        public ICommand ValueChangedCommand { get; set; }

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

        public RepeatType RepeatType { get; set; } = RepeatType.RepeatOnce; // todo: load from db

        public TimeSpan TrackPlaybackTime { get; set; }
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

        public TrackItem SelectedTrackItem { get; set; }
        public ObservableCollection<TrackItem> PlaylistTrackItems { get; set; } = new ObservableCollection<TrackItem>();
        public ObservableCollection<TrackItem> SelectedPlaylistTrackItems { get; set; } = new ObservableCollection<TrackItem>();
        public Models.Playlist.Playlist SelectedPlaylist { get; set; }

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

        public void SavePlayCount()
        {
            using var db = new AppDbContext();

            var play = db.Plays.FirstOrDefault(x => x.AddedDate == DateTime.Now.Date && x.TrackId == SelectedTrackItem.Track.Id);
            if (play != null)
            {
                play.PlayCount += 1;
                db.Plays.Update(play);
            }
            else
            {
                play = new Plays
                {
                    TrackId = SelectedTrackItem.Track.Id
                };

                db.Plays.Add(play);
            }

            db.SaveChanges();
        }

        public void CheckDailyMix()
        {

        }

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
            if (SelectedTrackItem != null)
            {
                if (NAudioPlayer.WaveOutEvent.PlaybackState == NAudio.Wave.PlaybackState.Stopped)
                {
                    PlayerTimer.Stop();
                }

                if (SelectedTrackItem != null)
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
            return SelectedTrackItem != null;
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
                    PlayTrackItem(GetPreviousTrackItem(PlaylistTrackItems, SelectedTrackItem));
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
                    PlayTrackItem(GetNextTrackItem(PlaylistTrackItems, SelectedTrackItem));
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
                // get in list item
                else
                {
                    var toggleButton = (sender as ToggleButton);

                    if (toggleButton.DataContext is TrackItem _trackitem)
                    {
                        trackItem = _trackitem;
                    }
                    else if (toggleButton.DataContext is PlaysItem playsItem)
                    {
                        trackItem = new TrackItem 
                        { 
                            Track = playsItem.Track
                        };
                    }
                }

                // if there is track
                if (trackItem.Track != null)
                {
                    if (SelectedTrackItem == trackItem)
                    {
                        NAudioPlayer.TogglePlayPause(CurrentVolume);
                    }
                    else
                    {
                        // if there is selected track set is playing false
                        if (SelectedTrackItem != null)
                        {
                            SelectedTrackItem.IsPlaying = false;
                            SelectedTrackItem = null;
                        }

                        // dispose previous player/track
                        NAudioPlayer.Dispose();

                        // set new track and play
                        SelectedTrackItem = trackItem;
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
        /// Set repeat type
        /// </summary>
        public void ChangeRepeatType()
        {
            if (RepeatType == RepeatType.Repeat)
            {
                RepeatType = RepeatType.RepeatOnce;
            }
            else if (RepeatType == RepeatType.RepeatOnce)
            {
                RepeatType = RepeatType.RepeatOff;
            }
            else if (RepeatType == RepeatType.RepeatOff)
            {
                RepeatType = RepeatType.Repeat;
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

        public void ValueChanged(object sender)
        {
            AudioPosition = NAudioPlayer.GetPosition();
            AudioPositionInSeconds = NAudioPlayer.GetPositionInSeconds();
            AudioRemaining = NAudioPlayer.GetRemaining();
        }

        /// <summary>
        /// Set volume
        /// </summary>
        public void VolumeChanged()
        {
            if (SelectedTrackItem != null)
            {
                NAudioPlayer.SetVolume(CurrentVolume);
            }
        }

        /// <summary>
        /// Add to playlist
        /// </summary>
        public void AddToPlaylist()
        {
            var dialog = new AddToPlaylistDialog();

            dialog.ShowDialogWindow(new AddToPlaylistViewModel(dialog, SelectedTrackItem.Track));
        }

        #endregion

        #endregion


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

            if (AudioLength == AudioPosition)
            {
                SavePlayCount();

                // if playlist is not null play next
                if (PlaylistTrackItems != null && PlaylistTrackItems.Count() > 0)
                {
                    if (RepeatType == RepeatType.RepeatOnce)
                    {
                        NAudioPlayer.SetPosition(0);
                        NAudioPlayer.Play(NAudio.Wave.PlaybackState.Stopped, CurrentVolume);
                    }
                    else
                    {
                        PlayTrackItem(GetNextTrackItem(PlaylistTrackItems, SelectedTrackItem));
                    }
                }
                else if (SelectedTrackItem != null)
                {
                    if (RepeatType == RepeatType.RepeatOnce)
                    {
                        NAudioPlayer.SetPosition(0);
                        NAudioPlayer.Play(NAudio.Wave.PlaybackState.Stopped, CurrentVolume);
                    }
                }
            }
        }

        /// <summary>
        /// NAudioPlayer stopped
        /// </summary>
        private void NAudioPlayer_PlaybackStopped()
        {
            // if playlist is not null play next
            if (PlaylistTrackItems != null && PlaylistTrackItems.Count() > 0)
            {
                if (RepeatType == RepeatType.RepeatOnce)
                {
                    StartPlayback(null);
                }
                else
                {
                    PlayTrackItem(GetNextTrackItem(PlaylistTrackItems, SelectedTrackItem));
                }
            }
            else if (SelectedTrackItem != null)
            {
                if (RepeatType == RepeatType.RepeatOnce)
                {
                    StartPlayback(null);
                }
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