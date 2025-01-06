using ActualizacionClientesIdealMaui.Services;
using ActualizacionClientesIdealMaui.Views;
using Application = Microsoft.Maui.Controls.Application;

namespace ActualizacionClientesIdealMaui
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            

            DependencyService.Register<MockDataStore>();
            DependencyService.Register<NavigationService>();
            Routing.RegisterRoute(typeof(ItemDetailPage).FullName, typeof(ItemDetailPage));
            Routing.RegisterRoute(typeof(SincronizarPage).FullName, typeof(SincronizarPage));
            Routing.RegisterRoute(nameof(AboutPage), typeof(AboutPage));

            Application.Current.UserAppTheme = AppTheme.Light;

            MainPage = new MainPage();

        }
    }
}