﻿using ActualizacionClientesIdealMaui.Models;
using DevExpress.Maui.DataForm;

namespace ActualizacionClientesIdealMaui.ViewModels
{
    public class NewItemViewModel : BaseViewModel
    {
        public const string ViewName = "SincronizarPage";


        string text;
        string description;

        public NewItemViewModel()
        {
            Title = "New Item";
            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(OnCancel);
            PropertyChanged +=
                (_, __) => SaveCommand.ChangeCanExecute();
        }


        public string Text
        {
            get => this.text;
            set => SetProperty(ref this.text, value);
        }

        public string Description
        {
            get => this.description;
            set => SetProperty(ref this.description, value);
        }


        [DataFormDisplayOptions(IsVisible = false)]
        public Command SaveCommand { get; }

        [DataFormDisplayOptions(IsVisible = false)]
        public Command CancelCommand { get; }


        bool ValidateSave()
        {
            return !String.IsNullOrWhiteSpace(this.text)
                && !String.IsNullOrWhiteSpace(this.description);
        }

        async void OnCancel()
        {
            // This will pop the current page off the navigation stack
            await Navigation.GoBackAsync();
        }

        async void OnSave()
        {
            Item newItem = new Item()
            {
                Id = Guid.NewGuid().ToString(),
                Text = Text,
                Description = Description
            };

            await DataStore.AddItemAsync(newItem);

            // This will pop the current page off the navigation stack
            await Navigation.GoBackAsync();
        }
    }
}