using ActualizacionClientesIdealMaui.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace ActualizacionClientesIdealMaui.ViewModels;
public partial class LoginViewModel : ObservableObject
{
     string ip = "http://201.222.61.165:8092/"; //produccion
    
    public LoginViewModel()
    {
        Preferences.Set("db", "AuroraGT");
        //Preferences.Set("db", "AuroraSV");
        ip = ip + Preferences.Get("db", "") + "/";
    }

    [ObservableProperty]
    string userName;

    [ObservableProperty]
    string password;

    [ObservableProperty]
    string resultText;

    public INavigationService Navigation => DependencyService.Get<INavigationService>();


    [RelayCommand]
    async Task ingreso()
    {
        
        if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Password))
        {
            ResultText = "Por favor ingrese usuario y contraseña.";
        }
        else
        {
            HttpClient client = new HttpClient();
            string link = ip + "validarusuario/" + UserName + "/" + Password;
            string json = client.GetStringAsync(link).Result;

            if (Convert.ToBoolean(json) == true)
            {

                Preferences.Set("ip", "http://201.222.61.165:8092/");
                Preferences.Set("usuario", UserName);
                Preferences.Set("pass", Password);
                Preferences.Set("logueado", true);

                //await Shell.Current.GoToAsync("//about");
                await Navigation.NavigateToAsync<DataGridViewModel>(true);
                //await _navigation.PushAsync(new AboutPage());
            }
            else
            {
                ResultText = "Usuario o contraseña incorrectos";
            }

        }
    }


}
