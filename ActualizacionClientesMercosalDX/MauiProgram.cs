using DevExpress.Maui;
using Microsoft.Maui.Controls.Compatibility.Hosting;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Maui;
using ActualizacionClientesIdealMaui.Views;
using ActualizacionClientesIdealMaui.ViewModels;

namespace ActualizacionClientesIdealMaui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseDevExpress(useLocalization: true)
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("roboto-regular.ttf", "Roboto");
                    fonts.AddFont("roboto-medium.ttf", "Roboto-Medium");
                    fonts.AddFont("roboto-bold.ttf", "Roboto-Bold");
                    fonts.AddFont("univia-pro-regular.ttf", "Univia-Pro");
                    fonts.AddFont("univia-pro-medium.ttf", "Univia-Pro Medium");
                })
                .ConfigureEffects((effects) =>
                {
                    effects.AddCompatibilityEffects(AppDomain.CurrentDomain.GetAssemblies());
                });
            DevExpress.Maui.Charts.Initializer.Init();
            DevExpress.Maui.CollectionView.Initializer.Init();
            DevExpress.Maui.Controls.Initializer.Init();
            DevExpress.Maui.Editors.Initializer.Init();
            DevExpress.Maui.DataGrid.Initializer.Init();
            DevExpress.Maui.Scheduler.Initializer.Init();

            builder.Services.AddTransient<AboutPage>();
            builder.Services.AddTransient<AboutViewModel>();
            
            return builder.Build();
        }
    }
}