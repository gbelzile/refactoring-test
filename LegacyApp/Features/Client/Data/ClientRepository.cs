using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace LegacyApp
{
    public sealed class ClientRepository : IClientRepository
    {
        public Client GetById(int id)
        {
            Client client = null;

            // Ici j'injecterais un IConfiguration ou encore mieux un IOptions<> typé.
            var connectionString = ConfigurationManager.ConnectionStrings["appDatabase"].ConnectionString;

            using var connection = new SqlConnection(connectionString);
            using var command = new SqlCommand
            {
                Connection = connection,
                CommandType = CommandType.StoredProcedure,
                CommandText = "uspGetClientById"
            };

            var parametr = new SqlParameter("@clientId", SqlDbType.Int) { Value = id };
            command.Parameters.Add(parametr);

            connection.Open();
            using var reader = command.ExecuteReader(CommandBehavior.CloseConnection);
            while (reader.Read())
            {
                client = new Client
                {
                    Id = int.Parse(reader["ClientId"].ToString()),
                    Name = reader["Name"].ToString(),
                    ClientStatus = (ClientStatus)int.Parse(reader["ClientStatus"].ToString())
                };
            }

            return client;
        }
    }
}