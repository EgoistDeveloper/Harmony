using Dna;
using Harmony.DI;
using Harmony.ViewModel.App;
using Microsoft.Extensions.DependencyInjection;

namespace Harmony.DI
{
    /// <summary>
    /// Extension methods for the <see cref="FrameworkConstruction"/>
    /// </summary>
    public static class FrameworkConstructionExtensions
    {
        public static FrameworkConstruction AddAppViewModels(this FrameworkConstruction construction)
        {
            // Bind to a single instance of Application view model
            construction.Services.AddSingleton<ApplicationViewModel>();

            // Return the construction for chaining
            return construction;
        }
    }
}
