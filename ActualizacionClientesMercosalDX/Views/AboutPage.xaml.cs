using ActualizacionClientesIdealMaui.ViewModels;

namespace ActualizacionClientesIdealMaui.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AboutPage : ContentPage
    {
        public AboutPage()
        {
            InitializeComponent();
            BindingContext = new AboutViewModel();
        }
    }
}