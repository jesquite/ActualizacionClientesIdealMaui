
using SQLite;

namespace ActualizacionClientesIdealMaui.Models
{
    public class Usuario
    {
        [PrimaryKey,AutoIncrement]
        public int Id { get; set; }
        public string user { get; set; }
        public string password { get; set; }
        public string asNombresUsuario { get; set; }
        public string asApellidosUsuario { get; set; }  

    }

    public class UsuarioL
    {
        public List<Usuario> usuarios { get; set; }
    }
}
