﻿using ActualizacionClientesIdealMaui.ViewModels;

namespace ActualizacionClientesIdealMaui.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChartsPage : ContentPage
    {
        public ChartsPage()
        {
            InitializeComponent();
            BindingContext = ViewModel = new ChartsViewModel();
        }

        ChartsViewModel ViewModel { get; }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel.OnAppearing();
        }
    }
}