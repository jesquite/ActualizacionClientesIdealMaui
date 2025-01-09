

using SQLite;

namespace ActualizacionClientesIdealMaui.Models
{
    public class Cliente
    {
        [PrimaryKey]
        public string ccCodigoCliente { get; set; }
        public string ccNombreCliente { get; set; }
        public string telefono { get; set; }
        public string ccEmail { get; set; }
        public string direccion { get; set; }
        public string dgNombreDepartamento { get; set; }
        public string dgNombreMunicipio { get; set; }
        public string ccNombreComercial { get; set; }
        public int ccCodigoClienteCategoria { get; set; }
        public string colonia { get; set; }
        public int dgCodigoDepartamento { get; set; }
        public int dgCodigoMunicipio { get; set; }
        public string domicilioFiscal { get; set; }
        public string fotoNegocio { get; set; }
        public string fotoNegocio2 { get; set; }
        public string estado { get; set; } = "0";//1 actualizado 0 pendiente
    
        
    }

    public class ClientesL
    {
        public List<Cliente> clientes { get; set; }
    }
}
