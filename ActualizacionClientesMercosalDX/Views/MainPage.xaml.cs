using ActualizacionClientesIdealMaui.Services;


namespace ActualizacionClientesIdealMaui.Views
{
    public partial class MainPage : Shell
    {
        public MainPage()
        {
            InitializeComponent();
            Application.Current.UserAppTheme = AppTheme.Light;
        }

        public INavigationService Navigation => DependencyService.Get<INavigationService>();

        async void OnMenuItemClicked(object sender, EventArgs e)
        {

            Preferences.Set("logueado", false);
            await Current.GoToAsync("//LoginPage");
        }

        void OnCloseClicked(object sender, EventArgs e)
        {
            Current.FlyoutIsPresented = false;
        }
    }
}