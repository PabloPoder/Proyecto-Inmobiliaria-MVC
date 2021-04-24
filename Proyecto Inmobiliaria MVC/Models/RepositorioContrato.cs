using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_Inmobiliaria_MVC.Models
{
    public class RepositorioContrato : RepositorioBase
    {
        public RepositorioContrato(IConfiguration configuration) : base(configuration)
        {

        }

        public int Alta(Contrato contrato)
        {
            int res = -1;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"INSERT INTO Contratos (FechaDesde,  FechaHasta, InquilinoId, InmuebleId) " +
                    $"VALUES (@FechaDesde, @FechaHasta, @InquilinoId, @InmuebleId);" +
                    "SELECT SCOPE_IDENTITY();"; // devuelve el id insertado (LAST_INSERT_ID para mysql)

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@FechaDesde", contrato.FechaDesde);
                    command.Parameters.AddWithValue("@FechaHasta", contrato.FechaHasta);
                    command.Parameters.AddWithValue("@InquilinoId", contrato.InquilinoId);
                    command.Parameters.AddWithValue("@InmuebleId", contrato.InmuebleId);

                    connection.Open();

                    res = Convert.ToInt32(command.ExecuteScalar()); // devuelve la primer columna de la primer fila de resultados del query (id)
                    contrato.Id = res;

                    connection.Close();
                }
            }
            return res;
        }

        public int Baja (int id)
        {
            int res = -1;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"UPDATE Contratos SET Estado = 0 WHERE id = @id;";

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

        public int Modificacion(Contrato contrato)
        {
            int res = -1;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"UPDATE Contratos SET FechaDesde = @FechaDesde, FechaHasta = @FechaHasta, " +
                    $"InquilinoId = @InquilinoId, InmuebleId = @InmuebleId " +
                    $"WHERE id = @id;";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@id", contrato.Id);
                    command.Parameters.AddWithValue("@FechaDesde", contrato.FechaDesde);
                    command.Parameters.AddWithValue("@FechaHasta", contrato.FechaHasta);
                    command.Parameters.AddWithValue("@InquilinoId", contrato.InquilinoId);
                    command.Parameters.AddWithValue("@InmuebleId", contrato.InmuebleId);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        public List<Contrato> ObtenerTodos()
        {
            List<Contrato> res = new List<Contrato>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT contrato.id, FechaDesde, FechaHasta, InquilinoId, InmuebleId, inquilino.Nombre, inquilino.Apellido, " +
                    $"inmueble.Direccion, inmueble.Ambientes, inmueble.Precio " +
                    $"FROM Contratos contrato " +
                    $"INNER JOIN Inquilinos inquilino ON contrato.InquilinoId = inquilino.id AND inquilino.Estado = 1 " +
                    $"INNER JOIN Inmuebles inmueble ON contrato.InmuebleId = inmueble.id AND inmueble.Estado = 1 " +
                    $"INNER JOIN Propietarios propietario ON inmueble.PropietarioId = propietario.Id AND propietario.Estado = 1 " +
                    $"WHERE contrato.Estado = 1;";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Contrato contrato = new Contrato
                        {
                            Id = reader.GetInt32(0),
                            FechaDesde = reader.GetDateTime(1),
                            FechaHasta = reader.GetDateTime(2),
                            InquilinoId = reader.GetInt32(3),
                            InmuebleId = reader.GetInt32(4),
                            Inquilino = new Inquilino
                            {
                                Id = reader.GetInt32(3),
                                Nombre = reader.GetString(5),
                                Apellido = reader.GetString(6),
                            },
                            Inmueble = new Inmueble
                            {
                                Id = reader.GetInt32(4),
                                Direccion = reader.GetString(7),
                                Ambientes = reader.GetInt32(8),
                                Precio = reader.GetDecimal(9),
                            }
                        };
                        res.Add(contrato);
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public Contrato ObtenerPorId(int id)
        {
            Contrato contrato = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT contrato.id, FechaDesde, FechaHasta, InquilinoId, InmuebleId, inquilinos.Nombre, inquilinos.Apellido, " +
                    $"inmuebles.Direccion, inmuebles.Ambientes, inmuebles.Precio " +
                    $"FROM Contratos contrato " +
                    $"INNER JOIN Inquilinos inquilinos ON contrato.InquilinoId = inquilinos.id " +
                    $"INNER JOIN Inmuebles inmuebles ON contrato.InmuebleId = inmuebles.id WHERE contrato.id = @id AND contrato.Estado = 1;";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();

                    if (reader.Read()) 
                    {
                        contrato = new Contrato
                        {
                            Id = reader.GetInt32(0),
                            FechaDesde = reader.GetDateTime(1),
                            FechaHasta = reader.GetDateTime(2),
                            InquilinoId = reader.GetInt32(3),
                            InmuebleId = reader.GetInt32(4),
                            Inquilino = new Inquilino
                            {
                                Id = reader.GetInt32(3),
                                Nombre = reader.GetString(5),
                                Apellido = reader.GetString(6),
                            },
                            Inmueble = new Inmueble
                            {
                                Id = reader.GetInt32(4),
                                Direccion = reader.GetString(7),
                                Ambientes = reader.GetInt32(8),
                                Precio = reader.GetDecimal(9),
                            }
                        };
                    }
                    connection.Close();
                }
            }
            return contrato;
        }

        public List<Contrato> ObtenerPorInmueble(int id)
        {
            List<Contrato> res = new List<Contrato>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT contrato.id, FechaDesde, FechaHasta, InquilinoId, InmuebleId, inquilinos.Nombre, inquilinos.Apellido, " +
                    $"inmuebles.Direccion, inmuebles.Ambientes " +
                    $"FROM Contratos contrato " +
                    $"INNER JOIN Inquilinos inquilinos ON contrato.InquilinoId = inquilinos.id " +
                    $"INNER JOIN Inmuebles inmuebles ON contrato.InmuebleId = inmuebles.id WHERE InmuebleId = @id AND contrato.Estado = 1;";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Contrato contrato = new Contrato
                        {
                            Id = reader.GetInt32(0),
                            FechaDesde = reader.GetDateTime(1),
                            FechaHasta = reader.GetDateTime(2),
                            InquilinoId = reader.GetInt32(3),
                            InmuebleId = reader.GetInt32(4),
                            Inquilino = new Inquilino
                            {
                                Id = reader.GetInt32(3),
                                Nombre = reader.GetString(5),
                                Apellido = reader.GetString(6),
                            },
                            Inmueble = new Inmueble
                            {
                                Id = reader.GetInt32(4),
                                Direccion = reader.GetString(7),
                                Ambientes = reader.GetInt32(8),
                            }
                        };
                        res.Add(contrato);
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public List<Contrato> ObtenerPorInquilino(int id)
        {
            List<Contrato> res = new List<Contrato>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT contrato.id, FechaDesde, FechaHasta, InquilinoId, InmuebleId, inquilinos.Nombre, inquilinos.Apellido, " +
                    $"inmuebles.Direccion, inmuebles.Ambientes " +
                    $"FROM Contratos contrato " +
                    $"INNER JOIN Inquilinos inquilinos ON contrato.InquilinoId = inquilinos.id " +
                    $"INNER JOIN Inmuebles inmuebles ON contrato.InmuebleId = inmuebles.id WHERE InquilinoId = @id AND contrato.Estado = 1;";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Contrato contrato = new Contrato
                        {
                            Id = reader.GetInt32(0),
                            FechaDesde = reader.GetDateTime(1),
                            FechaHasta = reader.GetDateTime(2),
                            InquilinoId = reader.GetInt32(3),
                            InmuebleId = reader.GetInt32(4),
                            Inquilino = new Inquilino
                            {
                                Id = reader.GetInt32(3),
                                Nombre = reader.GetString(5),
                                Apellido = reader.GetString(6),
                            },
                            Inmueble = new Inmueble
                            {
                                Id = reader.GetInt32(4),
                                Direccion = reader.GetString(7),
                                Ambientes = reader.GetInt32(8),
                            }
                        };
                        res.Add(contrato);
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public List<Contrato> ObtenerContratosVigentes()
        {
            List<Contrato> res = new List<Contrato>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT contrato.id, FechaDesde, FechaHasta, InquilinoId, InmuebleId, inquilino.Nombre, inquilino.Apellido, " +
                    $"inmueble.Direccion, inmueble.Ambientes " +
                    $"FROM Contratos contrato " +
                    $"INNER JOIN Inquilinos inquilino ON contrato.InquilinoId = inquilino.id AND inquilino.Estado = 1 " +
                    $"INNER JOIN Inmuebles inmueble ON contrato.InmuebleId = inmueble.id AND inmueble.Estado = 1 " +
                    $"INNER JOIN Propietarios propietario ON inmueble.PropietarioId = propietario.Id AND propietario.Estado = 1 " +
                    $"WHERE contrato.Estado = 1 AND FechaHasta >= GETDATE();";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Contrato contrato = new Contrato
                        {
                            Id = reader.GetInt32(0),
                            FechaDesde = reader.GetDateTime(1),
                            FechaHasta = reader.GetDateTime(2),
                            InquilinoId = reader.GetInt32(3),
                            InmuebleId = reader.GetInt32(4),
                            Inquilino = new Inquilino
                            {
                                Id = reader.GetInt32(3),
                                Nombre = reader.GetString(5),
                                Apellido = reader.GetString(6),
                            },
                            Inmueble = new Inmueble
                            {
                                Id = reader.GetInt32(4),
                                Direccion = reader.GetString(7),
                                Ambientes = reader.GetInt32(8),
                            }
                        };
                        res.Add(contrato);
                    }
                    connection.Close();
                }
            }
            return res;
        }
    }
}
