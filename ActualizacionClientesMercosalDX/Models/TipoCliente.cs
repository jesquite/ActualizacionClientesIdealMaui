
namespace ActualizacionClientesIdealMaui.Models
{
    public class TipoCliente
    {
        public int ccCodigoClienteTipo { get; set; }
        public string ccNombreClienteTipo { get; set; }

    }

    public class tiposclienteLista
    {
        public List<TipoCliente> ccClienteTipo { get; set; }
    }
}
