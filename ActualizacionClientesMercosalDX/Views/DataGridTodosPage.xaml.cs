﻿using ActualizacionClientesIdealMaui.Data;
using ActualizacionClientesIdealMaui.Mensajes;
using ActualizacionClientesIdealMaui.Models;
using ActualizacionClientesIdealMaui.Services;
using ActualizacionClientesIdealMaui.ViewModels;
using CommunityToolkit.Mvvm.Messaging;
using GoogleGson;
using System.Text.Json;

namespace ActualizacionClientesIdealMaui.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DataGridTodosPage : ContentPage
    {

        string ip = Preferences.Get("ip", "");

        dbClientesApp dbapp;
        string usuariou, nombreu, passw;

        public INavigationService Navigation => DependencyService.Get<INavigationService>();

        public DataGridTodosPage()
        {
            InitializeComponent();
            BindingContext = new DataGridViewModel();

            Application.Current.UserAppTheme = AppTheme.Light;
            dbapp = new dbClientesApp();

            usuariou = Preferences.Get("usuario", "");
            passw = Preferences.Get("pass", "");
            ip = ip + Preferences.Get("db", "") + "/";

            _ = iniciarCargaDatos();

            WeakReferenceMessenger.Default.Register<mensajeApp>(this, (remitente, mensaje) =>
            {
                if (mensaje.Value == "clienteactualizado")
                {
                    leerClientesLocal();
                }
                //else if(mensaje.Value =="obtenerDatosNuevos")
                //{
                //    obtenerClientesDeNuevo();
                //}

            });
        }

        private async Task iniciarCargaDatos()
        {
            try
            {
                Usuario uu = new Usuario();
                uu = await dbapp.GetUsuario();
                if (uu != null)
                {
                    if (uu.user == usuariou) //si es el mismo usuario ya logeado no carga nada mas que lo que ya tiene cargado
                    {
                        lblUsuario.Text = uu.asNombresUsuario;
                        nombreu = uu.asNombresUsuario;
                        await leerListaDatos();
                        await leerClientesLocal();
                        return;
                    }
                    //si es uno diferente entonces hará todo el proceso de nuevo

                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("error", ex.ToString(), "ac");
            }

        }

        private async void obtenerClientesDeNuevo()
        {
            HttpClient client = new HttpClient();

            string link = ip + "datosClientesUsuarioRutaAppGT/" + usuariou + "/" + passw;
            string json = client.GetStringAsync(link).Result;
            ClientesL clientes = JsonSerializer.Deserialize<ClientesL>(json);

            if (clientes.clientes.Count > 0)
            {
                await dbapp.deleteClientesAsync(); //limpia lista de clients en  base de datos local

                foreach (Cliente c in clientes.clientes)
                {
                    await dbapp.insertAsync(c);
                }

                await leerClientesLocal();
            }
        }

        private async Task leerListaDatos()
        {
            List<Departamento> d = new List<Departamento>();
            d = await dbapp.getDepartamentos();
            if (d.Count == 0)
            {
                string codPais = "1";
                HttpClient client = new HttpClient();
                string link = ip + "departamento/" + codPais + "/";
                string json = client.GetStringAsync(link).Result;

                departamentosL departamentos = JsonSerializer.Deserialize<departamentosL>(json);

                foreach (Departamento dd in departamentos.dgDepartamento)
                {
                    await dbapp.insertAsync(dd);
                }

                link = ip + "municipio/" + codPais + "/";
                json = client.GetStringAsync(link).Result;

                municipiosL municipios = JsonSerializer.Deserialize<municipiosL>(json);

                foreach (Municipio mm in municipios.dgMunicipio)
                {
                    await dbapp.insertAsync(mm);
                }

                link = ip + "giros/";
                json = client.GetStringAsync(link).Result;

                categoriasclienteLista girosL = JsonSerializer.Deserialize<categoriasclienteLista>(json);

                foreach (CategoriaCliente gg in girosL.ccClienteCategoria)
                {
                    await dbapp.insertAsync(gg);
                }
            }
        }

        private async Task leerClientesLocal()
        {
            List<Cliente> clientes = new List<Cliente>();
            clientes = await dbapp.getClientes();

            dgClientes.ItemsSource = clientes;

        }

        private void datagridClientes_DoubleTap(object sender, DevExpress.Maui.DataGrid.DataGridGestureEventArgs e)
        {
            var s = dgClientes.SelectedItem as Cliente;

            //MessagingCenter.Send<FacturasPage>(this, "factualChange");
            Preferences.Set("ccCodigoCliente", s.ccCodigoCliente.ToString());
            Preferences.Set("nombreusuario", nombreu);

            WeakReferenceMessenger.Default.Send(new mensajeApp("cliente_seleccionado"));


            Navigation.NavigateToAsync<ItemsViewModel>(true);
            //await Shell.Current.GoToAsync($"{nameof(FacturaActualPage)}?factura={s.numeroFactura.ToString()}");
            //await Shell.Current.GoToAsync(nameof(FacturaActualPage));
            //await Shell.Current.GoToAsync("factualRoute");
        }





        protected override bool OnBackButtonPressed()
        {

            Navigation.NavigateToAsync<DataGridViewModel>(true);
            return true;
        }


    }
}