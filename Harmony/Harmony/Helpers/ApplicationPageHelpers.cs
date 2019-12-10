using Harmony.Models.Common;
using Harmony.UI.Pages;
using Harmony.ViewModel.App;
using Harmony.ViewModel.Track;

namespace Harmony.Helpers
{
    /// <summary>
    /// Converts the <see cref="ApplicationPage"/> to an actual view/page
    /// </summary>
    public static class ApplicationPageHelpers
    {
        /// <summary>
        /// Takes a <see cref="ApplicationPage"/> and a view model, if any, and creates the desired page
        /// </summary>
        /// <param name="page"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public static BasePage ToBasePage(this ApplicationPage page, object viewModel = null)
        {
            // Find the appropriate page
            switch (page)
            {
                case ApplicationPage.Importer:
                    return new Importer();
                case ApplicationPage.Tracks:
                    return new Tracks();
                case ApplicationPage.Albums:
                    return new Albums();
                case ApplicationPage.Album:
                    return new Album();
                case ApplicationPage.Artists:
                    return new Artists();
                case ApplicationPage.Artist:
                    return new Artist();
                case ApplicationPage.Playlist:
                    return new Playlist();
                case ApplicationPage.History:
                    return new History();
                default:
                    // Debugger.Break();
                    return new WelcomePage();
            }
        }

        /// <summary>
        /// Converts a <see cref="BasePage"/> to the specific <see cref="ApplicationPage"/> that is for that type of page
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public static ApplicationPage ToApplicationPage(this BasePage page)
        {
            // Alert developer of issue
            //Debugger.Break();
            return default(ApplicationPage);
        }
    }
}
