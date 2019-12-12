using GalaSoft.MvvmLight;
using Harmony.Data;
using Harmony.Helpers;
using Harmony.Models.Player;
using Harmony.Models.Track;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Input;

namespace Harmony.ViewModel.Player
{
    public class ImporterViewModel : ViewModelBase
    {
        public ImporterViewModel()
        {
            ImportFolderCommand = new RelayCommand(p => ImportFolder());
            OpenFolderCommand = new RelayParameterizedCommand(OpenFolder);
            ReScanFolderCommand = new RelayParameterizedCommand(ReScanFolder);

            LoadLibraries();
        }

        #region Commands

        public ICommand OpenFolderCommand { get; set; }
        public ICommand ImportFolderCommand { get; set; }
        public ICommand ReScanFolderCommand { get; set; } // todo: implementation

        #endregion

        #region Properties

        public string ProcessOutput { get; set; }
        public bool IsProcessing { get; set; }
        public ObservableCollection<LibraryItem> LibraryItems { get; set; } = new ObservableCollection<LibraryItem>();

        #endregion

        #region Methods

        /// <summary>
        /// Load libraries from db
        /// </summary>
        public void LoadLibraries()
        {
            using var db = new AppDbContext();

            LibraryItems = db.Libraries.Where(x => x.FolderPath.Length > 0)
            .Select(library => new LibraryItem
            {
                Library = library,
                DirectoryInfo = new DirectoryInfo(library.FolderPath)
            }).ToObservableCollection();
        }

        /// <summary>
        /// Scan folder with target file formats
        /// </summary>
        /// <param name="baseDirectory"></param>
        /// <returns></returns>
        public IEnumerable<string> ScanFolder(string baseDirectory)
        {
            return Directory.EnumerateFiles(baseDirectory, "*.*", SearchOption.AllDirectories)
            .Where(x => x.EndsWith(".mp3") || x.EndsWith(".3g2") || x.EndsWith(".flac")
                || x.EndsWith(".3gp2") || x.EndsWith(".3gpp") || x.EndsWith(".3gp")
                || x.EndsWith(".asf") || x.EndsWith(".wma") || x.EndsWith(".wmv")
                || x.EndsWith(".aac") || x.EndsWith(".wma") || x.EndsWith(".adts")
                || x.EndsWith(".avi") || x.EndsWith(".wma") || x.EndsWith(".adts")
                || x.EndsWith(".m4a") || x.EndsWith(".m4a") || x.EndsWith(".m4a")
                || x.EndsWith(".mp4") || x.EndsWith(".wav") || x.EndsWith(".m4a"));
        }

        /// <summary>
        /// Add artists
        /// </summary>
        /// <param name="artists"></param>
        public void AddArtists(string[] artists)
        {
            using var db = new AppDbContext();

            foreach (var artist in artists)
            {
                if (!db.Artists.Any(x => x.FullName == artist))
                {
                    db.Artists.Add(new Artist 
                    {
                        FullName = artist
                    });
                }
            }

            db.SaveChanges();
        }

        /// <summary>
        /// Add album artists
        /// </summary>
        /// <param name="artists"></param>
        /// <param name="albumId"></param>
        public void AddAlbumArtists(string[] artists, long albumId)
        {
            using var db = new AppDbContext();

            foreach (var artist in artists)
            {
                var _artist = db.Artists.First(c => c.FullName == artist);

                if (!db.AlbumArtists.Any(x => x.AlbumId == albumId && x.ArtistId == _artist.Id))
                {
                    db.AlbumArtists.Add(new AlbumArtist
                    {
                        AlbumId = albumId,
                        ArtistId = _artist.Id
                    });
                }
            }

            db.SaveChanges();
        }

        /// <summary>
        /// Import files to db
        /// </summary>
        /// <param name="files"></param>
        public void ImportFiles(IEnumerable<string> files)
        {
            if (files.Count() > 0)
            {
                ProcessOutput = "Importing audio files...";

                var importedFiles = 0;

                new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;

                    using var db = new AppDbContext();

                    foreach (var file in files)
                    {
                        // if track is not exists in db
                        if (!db.Tracks.Any(x => x.FileLocation == file))
                        {
                            // get tag info
                            var tfile = TagLib.File.Create(file);
                            var artist = new Artist();
                            var album = new Album();
                            string[] artists = null;

                            // get or set  artist and save artists
                            if (tfile.Tag.Artists.Length > 0 || tfile.Tag.AlbumArtists.Length > 0)
                            {
                                string artistName = "";

                                if (tfile.Tag.Artists.Length > 0)
                                {
                                    artist = db.Artists.FirstOrDefault(x => x.FullName == tfile.Tag.Artists.First()) ?? 
                                    new Artist 
                                    { 
                                        FullName = tfile.Tag.Artists.First()
                                    };

                                    artists = tfile.Tag.Artists;
                                }
                                else if (tfile.Tag.AlbumArtists.Length > 0)
                                {
                                    artist = db.Artists.FirstOrDefault(x => x.FullName == tfile.Tag.AlbumArtists.First()) ??
                                    new Artist
                                    {
                                        FullName = tfile.Tag.AlbumArtists.First()
                                    };

                                    artists = tfile.Tag.AlbumArtists;
                                }

                                if (artist.Id <= 0)
                                {
                                    db.Artists.Add(artist);
                                    db.SaveChanges();
                                }

                                AddArtists(artists);
                            }

                            if (!string.IsNullOrEmpty(tfile.Tag.Album))
                            {
                                album = db.Albums.FirstOrDefault(x => x.Title == tfile.Tag.Album);

                                if (album == null)
                                {
                                    album = db.Albums.FirstOrDefault(x => x.Title == tfile.Tag.Album && x.ArtistId == artist.Id) ??
                                    new Album
                                    {
                                        Title = tfile.Tag.Album,
                                        ArtistId = artist.Id
                                    };

                                    if (album.Id <= 0)
                                    {
                                        db.Albums.Add(album);
                                        db.SaveChanges();
                                    }
                                }
                                
                                AddAlbumArtists(artists, album.Id);
                            }

                            var track = new Models.Track.Track
                            {
                                Title = tfile.Tag.Title,
                                FileLocation = file,
                                Time = tfile.Properties.Duration,
                                Description = tfile.Tag.Description,
                                Lyrics = tfile.Tag.Lyrics,
                                Image = tfile.Tag.Pictures[0]?.Data.Data.ByteArrayToBitmapImage(),
                                Disc = tfile.Tag.Disc,
                                Number = tfile.Tag.TrackCount,

                                ArtistId = artist.Id,
                                AlbumId = album.Id
                            };

                            // add new track
                            db.Tracks.Add(track);
                            db.SaveChanges();

                            importedFiles += 1;
                        }
                    }

                    // todo: may you need run in foreach
                    db.SaveChanges();

                    ProcessOutput = $"Process is done. Added {importedFiles} files.";
                }).Start();
            }
            else
            {
                ProcessOutput = "There is no audio files. Supported formats: .mp3, .3g2, .flac, .3gp2, .3gpp, .3gp, .asf, .wma, .wmv, .aac, .wma, .adts, .m4a, .m4a, .m4a, .mp4, .wav, .m4a";
            }
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// Open selected folder
        /// </summary>
        /// <param name="sender"></param>
        public void OpenFolder(object sender)
        {
            var libraryItem = (sender as System.Windows.Controls.Button).DataContext as LibraryItem;            

            Process.Start(libraryItem.Library.FolderPath);
        }

        /// <summary>
        /// Import selected folder
        /// </summary>
        public void ImportFolder()
        {
            ProcessOutput = "Selecting library folder...";

            var folderBrowserDialog = new FolderBrowserDialog
            {
                Description = "Select Library Folder",
                RootFolder = Environment.SpecialFolder.Desktop
            };

            var result = folderBrowserDialog.ShowDialog();
            if (result == DialogResult.OK && !string.IsNullOrEmpty(folderBrowserDialog.SelectedPath))
            {
                using var db = new AppDbContext();

                var library = new Library
                {
                    FolderPath = folderBrowserDialog.SelectedPath
                };

                if (!db.Libraries.Any(x => x.FolderPath == folderBrowserDialog.SelectedPath))
                {
                    db.Libraries.Add(library);
                    db.SaveChanges();

                    LibraryItems.Add(new LibraryItem
                    {
                        Library = library,
                        DirectoryInfo = new DirectoryInfo(folderBrowserDialog.SelectedPath)
                    });
                }

                var files = ScanFolder(folderBrowserDialog.SelectedPath);

                ImportFiles(files);
            }
            else
            {
                ProcessOutput = "Selecting failed or canceled";
            }
        }

        /// <summary>
        /// Recan and import selected folder
        /// </summary>
        /// <param name="sender"></param>
        public void ReScanFolder(object sender)
        {
            var libraryItem = ((System.Windows.Controls.Button)sender).DataContext as LibraryItem;

            var files = ScanFolder(libraryItem.Library.FolderPath);

            ImportFiles(files);
        }

        #endregion
    }
}