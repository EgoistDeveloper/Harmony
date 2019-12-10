/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:KOR.SysInfo.Core"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Harmony.ViewModel.App;
using Harmony.ViewModel.Player;
using Harmony.ViewModel.Playlist;
using Harmony.ViewModel.Track;
using static Harmony.DI.DI;
//using Microsoft.Practices.ServiceLocation;

namespace Harmony.ViewModel.Base
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModelBase.IsInDesignModeStatic)
            {
            }
            else
            {
                SimpleIoc.Default.Register<NavbarViewModel>();
                SimpleIoc.Default.Register<AlbumsViewModel>();
                SimpleIoc.Default.Register<AlbumViewModel>();
                SimpleIoc.Default.Register<ImporterViewModel>();
                SimpleIoc.Default.Register<ArtistsViewModel>();
                SimpleIoc.Default.Register<ArtistViewModel>();
                SimpleIoc.Default.Register<TracksViewModel>();
                SimpleIoc.Default.Register<PlaylistViewModel>();
                SimpleIoc.Default.Register<HistoryViewModel>();

            }
        }

        #region Public Properties

        /// <summary>
        /// Singleton instance of the locator
        /// </summary>
        public static ViewModelLocator Instance { get; private set; } = new ViewModelLocator();

        /// <summary>
        /// The application view model
        /// </summary>
        public ApplicationViewModel ApplicationViewModel => ViewModelApplication;

        #endregion

        public NavbarViewModel NavbarVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<NavbarViewModel>();
            }
        }

        public AlbumsViewModel AlbumsVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<AlbumsViewModel>();
            }
        }

        public AlbumViewModel AlbumVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<AlbumViewModel>();
            }
        }

        public ImporterViewModel ImporterVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ImporterViewModel>();
            }
        }

        public ArtistsViewModel ArtistsVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ArtistsViewModel>();
            }
        }

        public ArtistViewModel ArtistVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ArtistViewModel>();
            }
        }

        public TracksViewModel TracksVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<TracksViewModel>();
            }
        }

        public PlaylistViewModel PlaylistVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<PlaylistViewModel>();
            }
        }

        public HistoryViewModel HistoryVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<HistoryViewModel>();
            }
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}