using ActualizacionClientesIdealMaui.Data;
using ActualizacionClientesIdealMaui.Mensajes;
using ActualizacionClientesIdealMaui.Models;
using ActualizacionClientesIdealMaui.ViewModels;
using Android.Views;
using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace ActualizacionClientesIdealMaui.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SincronizarPage : ContentPage
    {
        dbClientesApp dbapp;
        string ip = Preferences.Get("ip", "");

        public SincronizarPage()
        {
            InitializeComponent();

            dbapp = new dbClientesApp();

            ip = ip + Preferences.Get("db", "") + "/";
        }



        private async void btnObtenerDatos_Clicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Confirmación", "¿Desea obtener datos del servidor nuevamente?", "Si", "No");
            if (answer)
            {
                WeakReferenceMessenger.Default.Send(new mensajeApp("obtenerDatosNuevos"));
                WeakReferenceMessenger.Default.Send(new mensajeApp("clienteactualizado"));
                await DisplayAlert("Aviso", "Lista de clientes actualizada", "Aceptar");
            }
        }

        private async void btnEnviarDatos_Clicked(object sender, EventArgs e)
        {

            List<Cliente> clientes = new List<Cliente>();
            clientes = await dbapp.getClientesTransmision();

            foreach (Cliente c in clientes)
            {
                //actualizarDatosCliente(c.ccCodigoCliente, c.ccNombreCliente, c.ccNombreComercial, c.direccion, c.colonia, c.dgCodigoDepartamento, c.dgCodigoMunicipio, c.ccEmail, c.domicilioFiscal, c.telefono, c.dgNombreMunicipio, c.dgNombreDistrito, c.dgCodigoTipoPersona.ToString(), c.identificacionTributaria, c.snContribuyente.ToString(), c.ccNumeroNRC, c.ccActividadEconomica, c.dgNombreDepartamento, c.ccNumeroDPI, Convert.ToInt32( c.ccCodigoClienteGiro), Convert.FromBase64String( c.imgDUI), Convert.FromBase64String(c.imgNRC));
                 actualizarDatosCliente(c);
            }
            
            DisplayAlert("Aviso", clientes.Count + " clientes actualizados", "Aceptar");
        }

        //private async void actualizarDatosCliente(string codigoCliente, string nombreCliente, string nombreComercial, string direccion, string colonia, string codigoDepartamento, string codigoMunicipio, string email, string domicilioFiscal, string telefono, string nombreMunicipio, string nombreDistrito, string codigoTipoPersona, string identificacionTributaria, string contribuyente, string numeroNRC, string actividadEconomica, string nombreDepto, string numeroDPI, int codClienteGiro, byte[] DUIimageBytes, byte[] NRCimageBytes)
        private async void actualizarDatosCliente(Cliente clienteparaActualizarLocal)
        {
            string confirmado = "0";
            string usuario = Preferences.Get("usuario", "");

            clienteparaActualizarLocal.estado = "Actualizado";

         
            try
            {

                Location loc = await GetCurrentLocationAsync();
                if (loc == null)
                {
                    loc.Latitude = -14.4545;
                    loc.Longitude = -14.4545;
                }

                HttpClient client = new HttpClient();

                var clienteParaAPI = new datosDeCliente
                {
                    codigoCliente = clienteparaActualizarLocal.ccCodigoCliente,
                    nombreComercial = clienteparaActualizarLocal.ccNombreComercial,
                    direccion = clienteparaActualizarLocal.direccion,
                    colonia = clienteparaActualizarLocal.colonia,
                    codigoDepartamento = clienteparaActualizarLocal.dgCodigoDepartamento,
                    codigoMunicipio = clienteparaActualizarLocal.dgCodigoMunicipio,
                    email = clienteparaActualizarLocal.ccEmail,
                    domicilioFiscal = clienteparaActualizarLocal.domicilioFiscal,
                    telefono = clienteparaActualizarLocal.telefono,
                    confirmado = Convert.ToInt32(confirmado),
                    usuario = usuario,
                    ccCodigoClienteTipo = clienteparaActualizarLocal.ccCodigoClienteTipo.ToString(),
                    latitud = loc.Latitude.ToString(),
                    longitud = loc.Longitude.ToString()
                };

                string jsonContent = JsonConvert.SerializeObject(clienteParaAPI);

                HttpContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, ip + "actualizarClienteIdealsa");
                request.Content = content;

                HttpResponseMessage response = await client.SendAsync(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                
                    if (clienteparaActualizarLocal.fotoNegocio != "")
                    {
                        saveIMG(Convert.FromBase64String(clienteparaActualizarLocal.fotoNegocio ), clienteparaActualizarLocal.ccCodigoCliente, "0");
                    }

                   // await DisplayAlert("Actualizado", "La información del cliente fue actualizada", "Aceptar");
                    await dbapp.updateTable(clienteparaActualizarLocal);
                }
                else
                {
                    clienteparaActualizarLocal.estado = "TRANSMISION";
                    await dbapp.updateTable(clienteparaActualizarLocal);
                    await DisplayAlert("Error al actualizar", "Error en transmision, utilice el boton 'Enviar datos pendientes al servidor' para probar despues. \n" + ip + response.Content.ReadAsStringAsync(), "Aceptar");
                }

                WeakReferenceMessenger.Default.Send(new mensajeApp("clienteactualizado"));

            }
            catch (Exception s)
            {
                await DisplayAlert("Error:", s.ToString(), "Aceptar");
            }
        }


        public class ImageUploadRequest
        {
            public string codigoCliente { get; set; }
            public byte[] datosImagen { get; set; }
        }

        private async void saveIMG(byte[] img, string codigoCliente, string tipo)
        {
            try
            {

                HttpClient client = new HttpClient();

                var requestObj = new ImageUploadRequest
                {
                    codigoCliente = codigoCliente,
                    datosImagen = img,
                };

                string jsonContent = JsonConvert.SerializeObject(requestObj);

                HttpContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, ip + "subirFotoNegocio");
                request.Content = content;

                HttpResponseMessage response = await client.SendAsync(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    //await DisplayAlert("", "API Response: " + response.Content.ToString(), "ok");
                }
                else
                {
                    await DisplayAlert("Error al guardar imagen", "API Request Failed: " + response.Content.ToString(), "Aceptar");
                }

            }
            catch (Exception ex)
            {
                await DisplayAlert("", "Error: " + ex.ToString(), "Ok");
            }
        }


        private async Task<Location> GetCurrentLocationAsync()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                Location location = await Geolocation.GetLocationAsync(request);
                return location;
            }
            catch (FeatureNotSupportedException)
            {
                // Handle not supported on device exception
                return null;
            }
            catch (FeatureNotEnabledException)
            {
                await DisplayAlert("", "La ubicación del dispositivo está desactivada, el proceso continuará pero debe activarla y conceder permiso.", "Aceptar");
                return null;
            }
            catch (PermissionException)
            {
                await DisplayAlert("", "No se tiene acceso a la ubicación del dispositivo, el proceso continuará pero debe conceder permiso.", "Aceptar");
                return null;
            }
            catch (Exception)
            {
                // Handle other exceptions
                return null;
            }
        }


    }
}