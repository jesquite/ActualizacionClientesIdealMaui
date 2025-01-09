
namespace ActualizacionClientesIdealMaui.Models
{
    public class CategoriaCliente
    {
        public int ccCodigoClienteCategoria { get; set; }
        public string ccNombreClienteCategoria { get; set; }

    }

    public class categoriasclienteLista
    {
        public List<CategoriaCliente> ccClienteCategoria { get; set; }
    }
}
