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
                string sql = $"INSERT INTO Pagos (FechaPago, ContratoId, Precio) " +
                             $"VALUES (@FechaPago, @ContratoId, @Precio); " +
                             $"SELECT SCOPE_IDENTITY()"; // devuelve el id insertado

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@FechaPago", pago.FechaPago);
                    command.Parameters.AddWithValue("@ContratoId", pago.ContratoId);
                    command.Parameters.AddWithValue("@Precio", pago.Precio);


                    connection.Open();

                    res = Convert.ToInt32(command.ExecuteScalar()); // devuelve la primer columna de la primer fila de resultados del query (id)
                    pago.Id = res;

                    connection.Close();
                }
            }
            return res;
        }

        public int Modificacion(Pago pago)
        {
            int res = -1;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"UPDATE Pagos SET FechaPago = @FechaPago, ContratoId = @ContratoId, " +
                    $"Estado = @Estado " +
                    $"WHERE id = @id";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@id", pago.Id);
                    command.Parameters.AddWithValue("@FechaPago", pago.FechaPago);
                    command.Parameters.AddWithValue("@ContratoId", pago.ContratoId);
                    command.Parameters.AddWithValue("@Estado", pago.Estado);

                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        public Pago ObtenerPorId(int id)
        {
            Pago pago = null;

            using(SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT pago.id, FechaPago, pago.Precio, ContratoId, contrato.InmuebleId, inmueble.Precio " +
                    $"FROM Pagos pago " +
                    $"INNER JOIN Contratos contrato ON pago.ContratoId = contrato.id " +
                    $"INNER JOIN Inmuebles inmueble ON contrato.InmuebleId = inmueble.id " +
                    $"WHERE pago.id = @id AND pago.Estado = 1;";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();

                    var reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        pago = new Pago
                        {
                            Id = reader.GetInt32(0),
                            FechaPago = reader.GetDateTime(1),
                            Precio = reader.GetDecimal(2),
                            ContratoId = reader.GetInt32(3),
                            Contrato = new Contrato
                            {
                                Id = reader.GetInt32(3),
                                InmuebleId = reader.GetInt32(4),
                                Inmueble = new Inmueble
                                {
                                    Id = reader.GetInt32(4),
                                    Precio = reader.GetDecimal(5),
                                }
                            }
                        };

                    }
                    connection.Close();
                }
            }
            return pago;
        }


        public List<Pago> ObtenerTodos(int id)
        {
            List<Pago> res = new List<Pago>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT pago.id, FechaPago, pago.Precio, ContratoId, contrato.InmuebleId, inmueble.Precio " +
                    $"FROM Pagos pago " +
                    $"INNER JOIN Contratos contrato ON pago.ContratoId = contrato.id " +
                    $"INNER JOIN Inmuebles inmueble ON contrato.InmuebleId = inmueble.id " +
                    $"WHERE contrato.id = @id AND pago.Estado = 1;";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Pago pago = new Pago
                        {
                            Id = reader.GetInt32(0),
                            FechaPago = reader.GetDateTime(1),
                            Precio = reader.GetDecimal(2),
                            ContratoId = reader.GetInt32(3),
                            Contrato = new Contrato
                            {
                                Id = reader.GetInt32(3),
                                InmuebleId = reader.GetInt32(4),
                                Inmueble = new Inmueble
                                {
                                    Id = reader.GetInt32(4),
                                    Precio = reader.GetDecimal(5),
                                }
                            }
                        };
                        res.Add(pago);
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public int Baja(int id)
        {
            int res = -1;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"UPDATE Pagos SET Estado = 0 WHERE id = @id;";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

    }
}
