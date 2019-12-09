using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using Microsoft.EntityFrameworkCore;
using System.Windows.Media.Imaging;
using Harmony.Helpers;
using Harmony.Models.AppSetting;
using Harmony.Models.Track;
using Harmony.Models.Playlist;
using Harmony.Models.Player;

namespace Harmony.Data
{
    public sealed class AppDbContext : DbContext
    {
        public AppDbContext()
        {
            Database.Migrate();
            if (!Database.EnsureCreated()) return;

            SaveChangesAsync();
        }

        #region Player

        public DbSet<Library> Libraries { get; set; }

        #endregion

        #region Track

        public DbSet<Track> Tracks { get; set; }
        public DbSet<Plays> Plays { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<AlbumArtist> AlbumArtists { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<TrackGenre> TrackGenres { get; set; }

        #endregion

        #region Playlist

        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<PlaylistTrack> PlaylistTracks { get; set; }
        public DbSet<PlaylistGroup> PlaylistGroups { get; set; }
        public DbSet<DailyMix> DailyMixes { get; set; }
        public DbSet<DailyMixTrack> DailyMixTracks { get; set; }

        #endregion

        #region AppSettings

        public DbSet<AppSetting> AppSettings { get; set; }

        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_sqlite3());
            optionsBuilder.UseSqlite("Data Source=Harmony.db;");
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Player

            // Player
            modelBuilder.Entity<Library>(entity =>
            {
                entity.HasKey(x => x.Id);
            });

            #endregion

            #region Track

            // Track
            modelBuilder.Entity<Track>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Image)
                    .HasConversion(c => c.ImageToByteArray(),
                        c => c.ByteArrayToBitmapImage());
                entity.Property(x => x.Time)
                    .HasConversion(c => c.ToString(),
                        c => TimeSpan.Parse(c));
            });

            // Plays
            modelBuilder.Entity<Plays>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.AddedDate)
                    .HasConversion(c => c.ToString("yyyy-MM-dd HH:mm:ss", Settings.CultureInfo),
                        c => DateTime.Parse(c));
            });

            // Artist
            modelBuilder.Entity<Artist>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Image)
                    .HasConversion(c => c.ImageToByteArray(),
                        c => c.ByteArrayToBitmapImage());
                entity.Property(x => x.Cover)
                    .HasConversion(c => c.ImageToByteArray(),
                        c => c.ByteArrayToBitmapImage());
            });

            // Album
            modelBuilder.Entity<Album>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.ReleaseDate)
                    .HasConversion(c => c.ToString("yyyy-MM-dd HH:mm:ss", Settings.CultureInfo),
                        c => DateTime.Parse(c));
                entity.Property(x => x.Image)
                    .HasConversion(c => c.ImageToByteArray(),
                        c => c.ByteArrayToBitmapImage());
            });

            // AlbumArtist
            modelBuilder.Entity<AlbumArtist>(entity =>
            {
                entity.HasKey(x => x.Id);
            });

            // Genre
            modelBuilder.Entity<Genre>(entity =>
            {
                entity.HasKey(x => x.Id);
            });

            // TrackGenre
            modelBuilder.Entity<TrackGenre>(entity =>
            {
                entity.HasKey(x => x.Id);
            });

            #endregion

            #region Playlist

            // Playlist
            modelBuilder.Entity<Playlist>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.AddedDate)
                    .HasConversion(c => c.ToString("yyyy-MM-dd HH:mm:ss", Settings.CultureInfo),
                        c => DateTime.Parse(c));
                entity.Property(x => x.Image)
                    .HasConversion(c => c.ImageToByteArray(),
                        c => c.ByteArrayToBitmapImage());
            });

            // PlaylistTrack
            modelBuilder.Entity<PlaylistTrack>(entity =>
            {
                entity.HasKey(x => x.Id);
            });

            // PlaylistGroup
            modelBuilder.Entity<PlaylistGroup>(entity =>
            {
                entity.HasKey(x => x.Id);
            });

            // DailyMix
            modelBuilder.Entity<DailyMix>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.AddedDate)
                    .HasConversion(c => c.ToString("yyyy-MM-dd HH:mm:ss", Settings.CultureInfo),
                        c => DateTime.Parse(c));
            });

            // DailyMixTrack
            modelBuilder.Entity<DailyMixTrack>(entity =>
            {
                entity.HasKey(x => x.Id);
            });

            #endregion

            #region AppSetting

            modelBuilder.Entity<AppSetting>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.IsEditable)
                    .HasConversion(c => Convert.ToInt32(c),
                        c => Convert.ToBoolean(c));
            });

            #endregion

            base.OnModelCreating(modelBuilder);
        }
    }
}