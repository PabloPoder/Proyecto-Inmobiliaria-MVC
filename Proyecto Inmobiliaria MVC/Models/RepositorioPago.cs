using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_Inmobiliaria_MVC.Models
{
    public class RepositorioPago : RepositorioBase
    {
        public RepositorioPago(IConfiguration configuration) : base(configuration)
        {
        }

        public int Alta(Pago pago)
        {
            int res = -1;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"INSERT INTO Pagos (FechaPago, ContratoId) " +
                             $"VALUES (@FechaDePago, @ContratoId); " +
                             $"SELCET SCOPE_IDENTITY()"; // devuelve el id insertado

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@FechaPago", pago.FechaPago);
                    command.Parameters.AddWithValue("@ContratoId", pago.ContratoId);

                    connection.Open();

                    res = Convert.ToInt32(command.ExecuteScalar()); // devuelve la primer columna de la primer fila de resultados del query (id)
                    pago.Id = res;

                    connection.Close();
                }
            }
            return res;
        }

        public List<Pago> ObtenerTodos(int id)
        {
            List<Pago> res = new List<Pago>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT pago.id, FechaPago " +
                    $"FROM Pagos pago " +
                    $"INNER JOIN Contratos contrato ON pago.ContratoId = contrato.id WHERE contrato.id = @id;";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@contrato.id", SqlDbType.Int).Value = id;
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Pago pago = new Pago
                        {
                            Id = reader.GetInt32(0),
                            FechaPago = reader.GetDateTime(1),
                            ContratoId = reader.GetInt32(3),
                        };
                        res.Add(pago);
                    }
                    connection.Close();
                }
            }
            return res;
        }


    }
}
