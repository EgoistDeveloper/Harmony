using NAudio.Wave;
using Remotion.Linq.Clauses.ResultOperators;
using System;
using System.IO;

namespace Harmony.Helpers
{
    public class NAudioPlayer
    {
        public NAudioPlayer()
        {
            PlaybackStopType = PlaybackStopTypes.PlaybackStoppedReachingEndOfFile;

            WaveOutEvent = new WaveOutEvent();
            WaveOutEvent.PlaybackStopped += Output_PlaybackStopped;
        }

        #region Events

        /// <summary>
        /// Playback paused event
        /// </summary>
        public event Action PlaybackPaused;

        /// <summary>
        /// Playback resume resumed event
        /// </summary>
        public event Action PlaybackResumed;

        /// <summary>
        /// Playback stopped event
        /// </summary>
        public event Action PlaybackStopped;

        #endregion

        #region Properties

        /// <summary>
        /// WaveOutEvent is NAudioPlayer player roperty
        /// </summary>
        public WaveOutEvent WaveOutEvent { get; set; } = new WaveOutEvent();

        /// <summary>
        /// File reader property
        /// </summary>
        public AudioFileReader AudioFileReader { get; set; }

        /// <summary>
        /// Playback stopped type
        /// </summary>
        public PlaybackStopTypes PlaybackStopType { get; set; }

        /// <summary>
        /// Is WaveOut imitalized
        /// </summary>
        public bool IsWaveOutEventInitalized { get; set; }

        /// <summary>
        /// Playback stop type
        /// </summary>
        public enum PlaybackStopTypes
        {
            PlaybackStoppedByUser,
            PlaybackStoppedReachingEndOfFile
        }

        #endregion

        #region Methods

        /// <summary>
        /// Play audio
        /// </summary>
        /// <param name="playbackState">Set Playback State</param>
        /// <param name="volume">Audio volume</param>
        /// <param name="audioFile">New audio ile</param>
        public void Play(PlaybackState playbackState, double volume, string audioFile = null)
        {
            if (playbackState == PlaybackState.Stopped || playbackState == PlaybackState.Paused)
            {
                if (audioFile != null && File.Exists(audioFile))
                {
                    AudioFileReader = new AudioFileReader(audioFile);

                    if (WaveOutEvent == null)
                    {
                        WaveOutEvent = new WaveOutEvent();
                    }

                    if (!IsWaveOutEventInitalized)
                    {
                        WaveOutEvent.Init(AudioFileReader);
                        IsWaveOutEventInitalized = true;
                    }

                    WaveOutEvent.Play();
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("audio file not found");
            }

            AudioFileReader.Volume = (float)volume;

            PlaybackResumed?.Invoke();
        }

        /// <summary>
        /// Pause audio
        /// </summary>
        public void Pause()
        {
            if (WaveOutEvent != null)
            {
                WaveOutEvent.Pause();

                PlaybackPaused?.Invoke();
            }
        }

        /// <summary>
        /// Toggle play pause audio
        /// </summary>
        /// <param name="volume"></param>
        public void TogglePlayPause(double volume)
        {
            if (WaveOutEvent != null)
            {
                if (WaveOutEvent.PlaybackState == PlaybackState.Playing)
                {
                    Pause();
                }
                else
                {
                    Play(WaveOutEvent.PlaybackState, volume);
                }
            }
            else
            {
                Play(PlaybackState.Stopped, volume);
            }
        }

        /// <summary>
        /// Stop audio
        /// </summary>
        public void Stop()
        {
            WaveOutEvent?.Stop();
        }

        /// <summary>
        /// Rewind audio
        /// </summary>
        public void Rewind(long before = 0)
        {
            AudioFileReader.Position = before > 0 && before > AudioFileReader.Position ? 
                before : 0;
        }

        /// <summary>
        /// Forward audio
        /// </summary>
        public void Forward(long after = 0)
        {
            var length = (long)GetLengthInSeconds();
            AudioFileReader.Position = after > 0 && AudioFileReader.Position - after > length - after ?
                length - after : length;
        }

        /// <summary>
        /// Dispose WaveOutEvent and AudioFileReader
        /// </summary>
        public void Dispose()
        {
            if (WaveOutEvent != null)
            {
                // if player is plaing
                if (WaveOutEvent.PlaybackState == PlaybackState.Playing)
                {
                    // stop first
                    WaveOutEvent.Stop();
                }

                WaveOutEvent.Dispose();
                WaveOutEvent = null;
            }

            if (AudioFileReader != null)
            {
                AudioFileReader.Dispose();
                AudioFileReader = null;
            }

            IsWaveOutEventInitalized = false;
        }

        /// <summary>
        /// Set audio position
        /// </summary>
        /// <param name="value"></param>
        public void SetPosition(double value)
        {
            if (AudioFileReader != null)
            {
                AudioFileReader.CurrentTime = TimeSpan.FromSeconds(value);
            }
        }

        /// <summary>
        /// Get audio volume
        /// </summary>
        /// <returns></returns>
        public float GetVolume()
        {
            return AudioFileReader != null ? AudioFileReader.Volume : 1;
        }

        /// <summary>
        /// Set volume
        /// </summary>
        /// <param name="value"></param>
        public void SetVolume(double value)
        {
            if (WaveOutEvent != null)
            {
                AudioFileReader.Volume = (float)value;
            }
        }

        #region Length - Duration - Remaining

        /// <summary>
        /// Get audio length as TimeSpan
        /// </summary>
        /// <returns></returns>
        public TimeSpan GetLength()
        {
            return AudioFileReader != null ? AudioFileReader.TotalTime : new TimeSpan(0);
        }

        /// <summary>
        /// Get audio length as double
        /// </summary>
        /// <returns></returns>
        public double GetLengthInSeconds()
        {
            return AudioFileReader != null ? AudioFileReader.TotalTime.TotalSeconds : 0;
        }

        /// <summary>
        /// Get current audio position as TimeSpan
        /// </summary>
        /// <returns></returns>
        public TimeSpan GetPosition()
        {
            return AudioFileReader != null ? AudioFileReader.CurrentTime : new TimeSpan(0);
        }

        /// <summary>
        /// Get current audio position as double
        /// </summary>
        /// <returns></returns>
        public double GetPositionInSeconds()
        {
            return AudioFileReader != null ? AudioFileReader.CurrentTime.TotalSeconds : 0;
        }

        /// <summary>
        /// Get current audio remaining as TimeSpan
        /// </summary>
        /// <returns></returns>
        public TimeSpan GetRemaining()
        {
            return GetLength() - GetPosition();
        }

        #endregion

        #region Get Playback States

        public bool IsPlaying()
        {
            if (WaveOutEvent != null && AudioFileReader != null)
            {
                return WaveOutEvent.PlaybackState == PlaybackState.Playing;
            }

            return false;
        }

        public bool IsPaused()
        {
            if (WaveOutEvent != null && AudioFileReader != null)
            {
                return WaveOutEvent.PlaybackState == PlaybackState.Paused;
            }

            return false;
        }

        public bool IsStopped()
        {
            if (WaveOutEvent != null && AudioFileReader != null)
            {
                return WaveOutEvent.PlaybackState == PlaybackState.Stopped;
            }

            return false;
        }

        #endregion

        #endregion

        #region Event Methods

        /// <summary>
        /// WaveOutEvent playback stopped event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Output_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            Dispose();
            PlaybackStopped?.Invoke();
        }

        #endregion
    }
}
