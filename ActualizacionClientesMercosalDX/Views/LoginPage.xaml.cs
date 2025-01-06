using ActualizacionClientesIdealMaui.ViewModels;
using ActualizacionClientesIdealMaui.Data;
using ActualizacionClientesIdealMaui.Models;
using ActualizacionClientesIdealMaui.Services;

namespace ActualizacionClientesIdealMaui.Views;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class LoginPage : ContentPage
{


    public INavigationService Navigation => DependencyService.Get<INavigationService>();

    public LoginPage()
    {
        InitializeComponent();
        BindingContext = new LoginViewModel();

        Application.Current.UserAppTheme = AppTheme.Light;

        bool logueado = Preferences.Get("logueado", false);

        if (logueado == true)
        {
            //Shell.Current.GoToAsync("//DataGridPage");
            Navigation.NavigateToAsync<DataGridViewModel>(true);
        }
    }


}


