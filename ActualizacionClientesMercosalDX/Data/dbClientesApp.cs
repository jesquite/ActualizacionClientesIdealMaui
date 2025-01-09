using ActualizacionClientesIdealMaui.Models;

using SQLite;

namespace ActualizacionClientesIdealMaui.Data
{
    public class dbClientesApp
    {
        SQLiteAsyncConnection dbconn;
        public dbClientesApp()
        {

        }

        async Task Init()
        {
            if (dbconn is not null)
                return;
            try
            {
                dbconn = new SQLiteAsyncConnection(Constants.DatabasePath);
                await dbconn.CreateTableAsync<Cliente>(); 
                await dbconn.CreateTableAsync<Departamento>();
                await dbconn.CreateTableAsync<CategoriaCliente>();
                await dbconn.CreateTableAsync<Municipio>();
                await dbconn.CreateTableAsync<Usuario>();

            }catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Cliente>> getClientes()
        {
            await Init();
            return await dbconn.Table<Cliente>().ToListAsync();
        }
        public async Task<List<Cliente>> getClientesPendientes()
        {
            await Init();
            return await dbconn.Table<Cliente>().Where(t=> t.estado == "Pendiente").ToListAsync();
        }
        public async Task<List<Cliente>> getClientesTransmision()
        {
            await Init();
            return await dbconn.Table<Cliente>().Where(t => t.estado == "TRANSMISION").ToListAsync();
        }
        public async Task<List<Departamento>> getDepartamentos()
        {
            await Init();
            return await dbconn.Table<Departamento>().ToListAsync();
        }

        public async Task<Departamento> getDepartamento(string nombre)
        {
            await Init();
            return await dbconn.Table<Departamento>().Where(t=> t.dgNombreDepartamento == nombre).FirstOrDefaultAsync();
        }

        public async Task<List<CategoriaCliente>> getCategoriasCliente()
        {
            await Init();
            return await dbconn.Table<CategoriaCliente>().ToListAsync();
        }

        public async Task<List<Municipio>> getMunicipios(int idDepartamento)
        {
            await Init();
            return await dbconn.Table<Municipio>().Where(t=> t.dgCodigoDepartamento == idDepartamento).ToListAsync();
        }
        public async Task<List<Municipio>> getTodosMunicipios()
        {
            await Init();
            return await dbconn.Table<Municipio>().ToListAsync();
        }
        public async Task<Usuario> GetUsuario()
        {
            await Init();
            return await dbconn.Table<Usuario>().FirstOrDefaultAsync();
        }

        public async Task<Cliente> getCliente(string id)
        {
            await Init();
            return await dbconn.Table<Cliente>().Where(t=> t.ccCodigoCliente == id).FirstOrDefaultAsync();
        }

        public async Task<int> insertAsync(object item)
        {
            await Init();
            return await dbconn.InsertAsync(item);
        }

        public async Task<int> updateTable(object item )
        {
            await Init();
            return await dbconn.UpdateAsync(item);
        }

        public async Task deleteClientesAsync()
        {
            await Init();
            await dbconn.DeleteAllAsync<Cliente>();
        }

        public async Task deleteUsuarioAsync()
        {
            await Init();
            await dbconn.DeleteAllAsync<Usuario>();
        }

        public async Task deleteAllTablesAsync()
        {
            await Init();
            await dbconn.DeleteAllAsync<Cliente>();
            await dbconn.DeleteAllAsync <Departamento>();
            await dbconn.DeleteAllAsync <CategoriaCliente>();
            await dbconn.DeleteAllAsync <Municipio>();
            await dbconn.DeleteAllAsync <Usuario>();
        }

    }
}
