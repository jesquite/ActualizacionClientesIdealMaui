using ActualizacionClientesIdealMaui.Data;
using ActualizacionClientesIdealMaui.Mensajes;
using ActualizacionClientesIdealMaui.Models;
using ActualizacionClientesIdealMaui.Services;
using ActualizacionClientesIdealMaui.ViewModels;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Graphics.Platform;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using static Android.Provider.ContactsContract;

namespace ActualizacionClientesIdealMaui.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public class datosDeCliente
    {
        public string codigoCliente { get; set; }
        public string nombreComercial { get; set; }
        public string direccion { get; set; }
        public string colonia { get; set; }
        public int codigoDepartamento { get; set; }
        public int codigoMunicipio { get; set; }
        public string email { get; set; }
        public string domicilioFiscal { get; set; }
        public string telefono { get; set; }
        public int confirmado { get; set; }
        public string usuario { get; set; }
        public string ccCodigoClienteCategoria { get; set; }
        public string latitud { get; set; }
        public string longitud { get; set; }

    }
    public partial class ItemsPage : ContentPage
    {
        dbClientesApp dbapp;
        Image fotoNegocio = new Image();
        byte[] fotoNegocioimageBytes;

        Image fotoNegocio2 = new Image();
        byte[] fotoNegocioimageBytes2;

        string codigo_cliente, codigoDepSeleccionado, codigoMuniSeleccionado ;
        int ccCodigoClienteCategoriaSeleccionado;
        List<Departamento> listaDepartamento;
        List<Municipio> listaMunicipio;
        List<CategoriaCliente> listaCategoriasCliente;


        string ip = Preferences.Get("ip", "");

        public ItemsPage()
        {
            InitializeComponent();
            BindingContext = new ItemsViewModel();
            Application.Current.UserAppTheme = AppTheme.Light;
            dbapp = new dbClientesApp();
            lblUsuario.Text = Preferences.Get("nombreusuario", "");
            ip = ip + Preferences.Get("db", "") + "/";

            _ = iniciarMostrarDatos();

            WeakReferenceMessenger.Default.Register<mensajeApp>(this, (remitente, mensaje) =>
            {
                if (mensaje.Value == "cliente_seleccionado")
                {
                    _ = obtenerdatosCliente();
                }
            });

        
        }

        private async Task iniciarMostrarDatos()
        {
            await llenarlistas();

            List<string> listaStringCategoriass = new List<string>();
            foreach (CategoriaCliente categoria in listaCategoriasCliente)
            {
                listaStringCategoriass.Add(categoria.ccNombreClienteCategoria);
            }
            pickercategoriacliente.ItemsSource = listaStringCategoriass;


            List<string> deptos = new List<string>();

            foreach (Departamento dd in listaDepartamento)
            {
                deptos.Add(dd.dgNombreDepartamento);
            }
            comboDepartamento.ItemsSource = deptos;

            obtenerdatosCliente();
        }

        private async Task llenarlistas()
        {
            listaMunicipio = new List<Municipio>();
            listaDepartamento = new List<Departamento>();
            listaCategoriasCliente = new List<CategoriaCliente>();

            listaMunicipio = await dbapp.getTodosMunicipios();
            listaCategoriasCliente = await dbapp.getCategoriasCliente();
            listaDepartamento = await dbapp.getDepartamentos();
        }


        private async Task obtenerdatosCliente()
        {
            codigo_cliente = Preferences.Get("ccCodigoCliente", "");
            if (codigo_cliente == "") return;

            txtCodigoCliente.Text = codigo_cliente;
            txtCodigoCliente.IsReadOnly = true;

            Cliente cliente = new Cliente();
            cliente = await dbapp.getCliente( codigo_cliente);

            txtNombreComercial.Text = cliente.ccNombreCliente;
            txtDireccion.Text = cliente.direccion;
            //if (cliente.direccion != cliente.domicilioFiscal) { chDomicilioFiscal.IsChecked = false; }
            //txtDomicilioFiscal.Text = cliente.domicilioFiscal == null ? "" : cliente.domicilioFiscal;
            txtCorreo.Text = cliente.ccEmail == null ? "" : cliente.ccEmail;
            txtTelefono.Text = cliente.telefono;
            comboDepartamento.SelectedItem = listaDepartamento.Find(t => t.dgCodigoDepartamento == cliente.dgCodigoDepartamento).dgNombreDepartamento;
            pickermunicipio.SelectedItem = listaMunicipio.Find(t => t.dgCodigoMunicipio == cliente.dgCodigoMunicipio).dgNombreMunicipio;
            txtColonia.Text = cliente.colonia;
            //txtDistrito.Text = c.dgNombreDistrito;

            imgDUI.IsVisible = false;
            if (cliente.fotoNegocio != null)
            {
                fotoNegocio.Source = convertirByteAImagen(convertirStringToByte(cliente.fotoNegocio));
                imgDUI.IsVisible = true;
            }

            img2.IsVisible = false;
            if (cliente.fotoNegocio2 != null)
            {
                fotoNegocio2.Source = convertirByteAImagen(convertirStringToByte(cliente.fotoNegocio2));
                img2.IsVisible = true;
            }

            //- es contribuyente
            foreach (CategoriaCliente categoria in listaCategoriasCliente)
            {
                if (categoria.ccCodigoClienteCategoria == cliente.ccCodigoClienteCategoria)
                {
                    pickercategoriacliente.SelectedItem = categoria.ccNombreClienteCategoria;
                    break;
                }
            }

        }


        private byte[] convertirStringToByte(string archivoImg)
        {
            byte[] imageBytes = Convert.FromBase64String(archivoImg);
            return imageBytes;
        }

        private ImageSource convertirByteAImagen(byte[] imageBytes)
        {
            MemoryStream memoryStream = new MemoryStream(imageBytes);
            
            return ImageSource.FromStream(() => memoryStream);
            

        }


        private async void evaluarDatosIngresados()
        {
            //actualizarDatosCliente();
            //return;
            List<string> datos = new List<string>();

            if (txtCodigoCliente.Text.Length == 0) datos.Add("Código de cliente");
            if (txtTelefono.Text.Length == 0) datos.Add("Teléfono");
            
            if (fotoNegocio.Source == null) datos.Add("Fotografía");
            if (pickercategoriacliente.SelectedItem == null) datos.Add("Categoría del cliente");

            if (datos.Count == 0)
            {
                actualizarDatosCliente();
            }
            else
            {
                string m = "Por favor complete los siguientes datos:\n";
                foreach (string s in datos)
                {
                    m += "-" + s + "\n";
                }
                await DisplayAlert("Datos incompletos", m, "Aceptar");
            }

        }


        private async void actualizarDatosCliente()
        {
            string codPais = "1";
            string codigoCliente = txtCodigoCliente.Text;
            string nombreComercial = txtNombreComercial.Text.Length > 0 ? txtNombreComercial.Text : "0";
            string direccion = txtDireccion.Text;
            string colonia = txtColonia.Text == "" ? "0" : txtColonia.Text;
            string codigoDepartamento = codigoDepSeleccionado;
            string codigoMunicipio = codigoMuniSeleccionado;
            string email = txtCorreo.Text == "" ? "0" : txtCorreo.Text;
            string domicilioFiscal = "0";//txtDomicilioFiscal.Text == "" ? "0" : txtDomicilioFiscal.Text;            
            string telefono = txtTelefono.Text.Length > 0 ? txtTelefono.Text : "0";
            string nombreMunicipio = pickermunicipio.SelectedItem.ToString();
            string confirmado = "0";
            string usuario = Preferences.Get("usuario", "");
            string nombreDepto = comboDepartamento.SelectedItem.ToString();
            
            Cliente cNombreCliente = await dbapp.getCliente(codigoCliente);

            Cliente clienteparaActualizarLocal = new Cliente();
            clienteparaActualizarLocal.ccCodigoCliente = codigoCliente;
            clienteparaActualizarLocal.ccNombreCliente = cNombreCliente.ccNombreCliente;
            clienteparaActualizarLocal.ccNombreComercial = nombreComercial;
            clienteparaActualizarLocal.direccion = direccion;
            clienteparaActualizarLocal.colonia = colonia;
            clienteparaActualizarLocal.dgCodigoDepartamento = Convert.ToInt32(codigoDepartamento);
            clienteparaActualizarLocal.dgCodigoMunicipio = Convert.ToInt32(codigoMunicipio);
            clienteparaActualizarLocal.ccEmail = email;
            clienteparaActualizarLocal.domicilioFiscal = domicilioFiscal;
            clienteparaActualizarLocal.telefono = telefono;
            clienteparaActualizarLocal.dgNombreMunicipio = nombreMunicipio;
            clienteparaActualizarLocal.dgNombreDepartamento = nombreDepto;
            clienteparaActualizarLocal.fotoNegocio = fotoNegocioimageBytes != null ? Convert.ToBase64String( fotoNegocioimageBytes) : "";
            clienteparaActualizarLocal.estado = "Actualizado";
            clienteparaActualizarLocal.ccCodigoClienteCategoria = ccCodigoClienteCategoriaSeleccionado;

            WeakReferenceMessenger.Default.Send(new mensajeApp("clienteactualizado"));

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
                    direccion =clienteparaActualizarLocal.direccion,
                    colonia = clienteparaActualizarLocal.colonia,
                    codigoDepartamento = clienteparaActualizarLocal.dgCodigoDepartamento,
                    codigoMunicipio = clienteparaActualizarLocal.dgCodigoMunicipio,
                    email = clienteparaActualizarLocal.ccEmail,
                    domicilioFiscal = clienteparaActualizarLocal.domicilioFiscal,
                    telefono = clienteparaActualizarLocal.telefono,
                    confirmado = Convert.ToInt32(confirmado),
                    usuario = usuario,
                    ccCodigoClienteCategoria = clienteparaActualizarLocal.ccCodigoClienteCategoria.ToString() ,
                    latitud = loc.Latitude.ToString(),
                    longitud = loc.Longitude.ToString()
                };

                string jsonContent = JsonConvert.SerializeObject(clienteParaAPI);

                indicadorActividad.IsVisible = true;
                indicadorActividad.IsRunning = true;
                btnIngresardatos.IsEnabled = false;

                HttpContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, ip + "actualizarClienteIdealsa");
                request.Content = content;

                HttpResponseMessage response = await client.SendAsync(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    if (fotoNegocioimageBytes != null)
                    {
                        await saveIMG(fotoNegocioimageBytes, codigoCliente, 1);
                    }

                    if (fotoNegocioimageBytes2 != null)
                    {
                        await saveIMG(fotoNegocioimageBytes2, codigoCliente, 2);
                    }

                    await DisplayAlert("Actualizado", "La información del cliente fue actualizada", "Aceptar");
                    await dbapp.updateTable(clienteparaActualizarLocal);

                    indicadorActividad.IsVisible = false;
                    indicadorActividad.IsRunning = false;
                    btnIngresardatos.IsEnabled = true;
                }
                else
                {
                    clienteparaActualizarLocal.estado = "TRANSMISION";
                    await dbapp.updateTable(clienteparaActualizarLocal);
                    await DisplayAlert("Error al actualizar", "Error en transmision, utilice el boton 'Enviar datos pendientes al servidor' para probar despues. \n"+ ip + response.Content.ReadAsStringAsync(), "Aceptar");

                    indicadorActividad.IsVisible = false;
                    indicadorActividad.IsRunning = false;
                    btnIngresardatos.IsEnabled = true;
                }

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
            public int numeroImagen { get; set; }
        }

        private async Task saveIMG(byte[] img, string codigoCliente, int numeroImagen)
        {
            try
            {

                HttpClient client = new HttpClient();

                var requestObj = new ImageUploadRequest
                {
                    codigoCliente = codigoCliente,
                    datosImagen = img,
                    numeroImagen = numeroImagen
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
                    await DisplayAlert("Error al guardar imagen", "La fotografía no pudo guardarse, intente utilizar una imagen con menos resolución (puede tomar la fotografía haciendo zoom para reducir la resolucion), los demas datos del cliente fueron guardados correctamente", "Aceptar");
                    //"API Request Failed: " + response.Content.ToString(), "Aceptar");
                }

            }
            catch (Exception ex)
            {
                await DisplayAlert("Error","Error: " + ex.ToString(),"Aceptar");
            }
        }



        private void OnSelectImageClicked(object sender, EventArgs e)
        {
            cargarImg(1, 0);
        }

        private void OnTakePictureClicked(object sender, EventArgs e)
        {
            takePicture();
        }

        private void OnTakePictureClicked2(object sender, EventArgs e)
        {
            takePicture2();
        }




        private async void cargarImg(int duif, int nrcf)
        {
            try
            {

                var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Seleccione la imagen"
                });
                var stream = await result.OpenReadAsync();
                var source = ImageSource.FromStream(() => stream);
                string path = result.FullPath;
                byte[] imageData = File.ReadAllBytes(path);

                if (duif == 1)
                {
                    fotoNegocioimageBytes = imageData;
                    fotoNegocio.Source = source;
                    imgDUI.IsVisible = true;
                }

            }
            catch (Exception ex)
            {
                // Handle exceptions
            }
        }


        private async Task takePicture()
        {
            try
            {
                if (MediaPicker.Default.IsCaptureSupported)
                {

                    FileResult photo = await MediaPicker.Default.CapturePhotoAsync();

                    if (photo != null)
                    {
                        string localFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);

                        using Stream sourceStream = await photo.OpenReadAsync();

                        using (FileStream localFileStream = File.OpenWrite(localFilePath))
                        {
                            await sourceStream.CopyToAsync(localFileStream);
                        };

                        byte[] imageData = File.ReadAllBytes(localFilePath);

                        //var image = PlatformImage.FromStream(sourceStream);
                        //image = image.Downsize(800, true);
                        // TODO
                        //resize image - se encontro que al cargar imagenes no las estaba enviando al db, puede ser ese el inconveniente, de igual forma es necesraio controlar el tamaño de la imgagen automaticamente para evitar el error y necesitar de la itervenciond el usuario


                        fotoNegocio.Source = ImageSource.FromFile(localFilePath);
                        fotoNegocioimageBytes = imageData;
                        imgDUI.IsVisible = true;


                    }
                }

            }
            catch (Exception ex)
            {
                // Handle exceptions
            }
        }

        private async Task takePicture2()
        {
            try
            {
                if (MediaPicker.Default.IsCaptureSupported)
                {

                    FileResult photo = await MediaPicker.Default.CapturePhotoAsync();

                    if (photo != null)
                    {
                        string localFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);

                        using Stream sourceStream = await photo.OpenReadAsync();

                        using (FileStream localFileStream = File.OpenWrite(localFilePath))
                        {
                            await sourceStream.CopyToAsync(localFileStream);
                        };

                        byte[] imageData = File.ReadAllBytes(localFilePath);

                        //var image = PlatformImage.FromStream(sourceStream);
                        //image = image.Downsize(800, true);
                        // TODO
                        //resize image - se encontro que al cargar imagenes no las estaba enviando al db, puede ser ese el inconveniente, de igual forma es necesraio controlar el tamaño de la imgagen automaticamente para evitar el error y necesitar de la itervenciond el usuario


                        fotoNegocio2.Source = ImageSource.FromFile(localFilePath);
                        fotoNegocioimageBytes2 = imageData;
                        img2.IsVisible = true;


                    }
                }

            }
            catch (Exception ex)
            {
                // Handle exceptions
            }
        }

        private void viewDUIImage(object sender, EventArgs e)
        {
            verImagen(fotoNegocio);
        }

        private void img2_Clicked(object sender, EventArgs e)
        {
            verImagen(fotoNegocio2);
        }
        private void chDomicilioFiscal_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            //if (chDomicilioFiscal.IsChecked == true)
            //{
            //    txtDomicilioFiscal.IsVisible = false;
            //}
            //else
            //{
            //    txtDomicilioFiscal.IsVisible = true;
            //}
        }

        private async void verImagen(Image img)
        {
            try
            {

                if (img.Source != null)
                {
                    var imageStream = await GetImageStreamAsync(img.Source);
                    if (imageStream != null)
                    {
                        var filePath = Path.Combine(FileSystem.CacheDirectory, "temp_image.jpg");
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageStream.CopyToAsync(fileStream);
                        }

                        await Launcher.OpenAsync(new OpenFileRequest
                        {
                            File = new ReadOnlyFile(filePath)
                        });
                    }
                }

            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.ToString(), "Aceptar");
            }
        }

        private async Task<Stream> GetImageStreamAsync(ImageSource imageSource)
        {
            if (imageSource is StreamImageSource streamImageSource)
            {
                return await streamImageSource.Stream(CancellationToken.None);
            }
            else if (imageSource is FileImageSource fileImageSource)
            {
                var imageFilePath = Path.Combine(FileSystem.CacheDirectory, fileImageSource.File);
                if (File.Exists(imageFilePath))
                {
                    return File.OpenRead(imageFilePath);
                }
            }

            return null;
        }



        private void comboDepartamento_SelectedIndexChanged(object sender, EventArgs e)
        {
            int codigoDep = 0;
            foreach (Departamento de in listaDepartamento)
            {
                if (de.dgNombreDepartamento == comboDepartamento.SelectedItem.ToString())
                {
                    codigoDep = de.dgCodigoDepartamento;
                    codigoDepSeleccionado = de.dgCodigoDepartamento.ToString();
                }
            }

            List<string> munis = new List<string>();

            foreach (Municipio mu in listaMunicipio)
            {
                if (mu.dgCodigoDepartamento == codigoDep)
                {
                    munis.Add(mu.dgNombreMunicipio);
                    codigoMuniSeleccionado = mu.dgCodigoMunicipio.ToString();
                }
            }

            pickermunicipio.ItemsSource = null;
            pickermunicipio.ItemsSource = munis;


        }

        private void btnIngresardatos_Clicked(object sender, EventArgs e)
        {
            evaluarDatosIngresados();
        }

        public INavigationService Navigation => DependencyService.Get<INavigationService>();

        private void pickercategoriacliente_SelectionChanged_1(object sender, EventArgs e)
        {

            foreach (CategoriaCliente categoria in listaCategoriasCliente)
            {
                if (categoria.ccNombreClienteCategoria == pickercategoriacliente.SelectedItem.ToString())
                {
                    ccCodigoClienteCategoriaSeleccionado = categoria.ccCodigoClienteCategoria;
                }
            }
        }


        protected override bool OnBackButtonPressed()
        {

            Navigation.NavigateToAsync<DataGridViewModel>(true);
            return true;
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