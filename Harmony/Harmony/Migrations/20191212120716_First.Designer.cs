﻿// <auto-generated />
using System;
using Harmony.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Harmony.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20191212120716_First")]
    partial class First
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity("Harmony.Models.AppSetting.AppSetting", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("DefaultValue");

                    b.Property<int>("IsEditable");

                    b.Property<string>("SettingName")
                        .IsRequired();

                    b.Property<string>("Value")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("AppSettings");
                });

            modelBuilder.Entity("Harmony.Models.Player.Library", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("FolderPath");

                    b.HasKey("Id");

                    b.ToTable("Libraries");
                });

            modelBuilder.Entity("Harmony.Models.Playlist.DailyMix", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AddedDate")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("DailyMixes");
                });

            modelBuilder.Entity("Harmony.Models.Playlist.DailyMixTrack", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("DailyMixId");

                    b.Property<long>("TrackId");

                    b.HasKey("Id");

                    b.HasIndex("DailyMixId");

                    b.ToTable("DailyMixTracks");
                });

            modelBuilder.Entity("Harmony.Models.Playlist.Playlist", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AddedDate")
                        .IsRequired();

                    b.Property<string>("Description");

                    b.Property<byte[]>("Image");

                    b.Property<long>("PlaylistGroupId");

                    b.Property<string>("Title")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("PlaylistGroupId");

                    b.ToTable("Playlists");
                });

            modelBuilder.Entity("Harmony.Models.Playlist.PlaylistGroup", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Title")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("PlaylistGroups");
                });

            modelBuilder.Entity("Harmony.Models.Playlist.PlaylistTrack", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("PlaylistId");

                    b.Property<long>("TrackId");

                    b.HasKey("Id");

                    b.HasIndex("TrackId");

                    b.ToTable("PlaylistTracks");
                });

            modelBuilder.Entity("Harmony.Models.Track.Album", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("ArtistId");

                    b.Property<string>("Description");

                    b.Property<byte[]>("Image");

                    b.Property<string>("ReleaseDate")
                        .IsRequired();

                    b.Property<string>("Title")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Albums");
                });

            modelBuilder.Entity("Harmony.Models.Track.AlbumArtist", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("AlbumId");

                    b.Property<long>("ArtistId");

                    b.HasKey("Id");

                    b.ToTable("AlbumArtists");
                });

            modelBuilder.Entity("Harmony.Models.Track.Artist", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<byte[]>("Cover");

                    b.Property<string>("FullName")
                        .IsRequired();

                    b.Property<byte[]>("Image");

                    b.Property<string>("Twitter");

                    b.Property<string>("Website");

                    b.HasKey("Id");

                    b.ToTable("Artists");
                });

            modelBuilder.Entity("Harmony.Models.Track.Genre", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("GenreName");

                    b.HasKey("Id");

                    b.ToTable("Genres");
                });

            modelBuilder.Entity("Harmony.Models.Track.Plays", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AddedDate")
                        .IsRequired();

                    b.Property<int>("PlayCount");

                    b.Property<long>("TrackId");

                    b.HasKey("Id");

                    b.ToTable("Plays");
                });

            modelBuilder.Entity("Harmony.Models.Track.Track", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("AlbumId");

                    b.Property<long>("ArtistId");

                    b.Property<string>("Description");

                    b.Property<uint>("Disc");

                    b.Property<string>("FileLocation")
                        .IsRequired();

                    b.Property<byte[]>("Image");

                    b.Property<string>("Lyrics");

                    b.Property<uint>("Number");

                    b.Property<string>("Time")
                        .IsRequired();

                    b.Property<string>("Title")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Tracks");
                });

            modelBuilder.Entity("Harmony.Models.Track.TrackGenre", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("GenreId");

                    b.Property<long>("TrackId");

                    b.HasKey("Id");

                    b.HasIndex("GenreId");

                    b.ToTable("TrackGenres");
                });

            modelBuilder.Entity("Harmony.Models.Playlist.DailyMixTrack", b =>
                {
                    b.HasOne("Harmony.Models.Playlist.DailyMix", "DailyMix")
                        .WithMany()
                        .HasForeignKey("DailyMixId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Harmony.Models.Playlist.Playlist", b =>
                {
                    b.HasOne("Harmony.Models.Playlist.PlaylistGroup", "PlaylistGroup")
                        .WithMany()
                        .HasForeignKey("PlaylistGroupId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Harmony.Models.Playlist.PlaylistTrack", b =>
                {
                    b.HasOne("Harmony.Models.Track.Track", "Track")
                        .WithMany()
                        .HasForeignKey("TrackId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Harmony.Models.Track.TrackGenre", b =>
                {
                    b.HasOne("Harmony.Models.Track.Genre", "Genre")
                        .WithMany()
                        .HasForeignKey("GenreId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
