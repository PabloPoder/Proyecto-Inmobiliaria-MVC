using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_Inmobiliaria_MVC.Models
{
    public class RepositorioInmueble : RepositorioBase
    {
        public RepositorioInmueble(IConfiguration configuration) : base(configuration)
        {

        }

        public int Alta(Inmueble inmueble)
        {
            int res = -1;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"INSERT INTO Inmuebles (Direccion, Ambientes, Superficie, Latitud, Longitud, Precio, PropietarioId) " +
                    $"VALUES (@direccion, @ambientes, @superficie, @latitud, @longitud, @precio, @propietarioId); " +
                    "SELECT SCOPE_IDENTITY();"; // devuelve el id insertado (LAST_INSERT_ID para mysql)

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@direccion", inmueble.Direccion);
                    command.Parameters.AddWithValue("@ambientes", inmueble.Ambientes);
                    command.Parameters.AddWithValue("@superficie", inmueble.Superficie);
                    command.Parameters.AddWithValue("@latitud", inmueble.Longitud);
                    command.Parameters.AddWithValue("@longitud", inmueble.Longitud);
                    command.Parameters.AddWithValue("@precio", inmueble.Precio);
                    command.Parameters.AddWithValue("@propietarioId", inmueble.PropietarioId);

                    connection.Open();

                    res = Convert.ToInt32(command.ExecuteScalar()); // devuelve la primer columna de la primer fila de resultados del query (id)
                    inmueble.Id = res;

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
                string sql = "UPDATE Inmuebles SET Estado = 0 WHERE id = @id";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    res = command.ExecuteNonQuery(); // devuelve el numero de filas afectadas 
                    connection.Close();
                }
            }
            return res;
        }

        public int Modificacion(Inmueble inmueble)
        {
            int res = -1;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"UPDATE Inmuebles SET Direccion = @direccion, Ambientes = @ambientes, " +
                    $"superficie = @superficie, Latitud = @latitud, Longitud = @longitud, Precio = @precio, " +
                    $"PropietarioId = @propietarioId " +
                    $"WHERE id = @id";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@id", inmueble.Id);
                    command.Parameters.AddWithValue("@direccion", inmueble.Direccion);
                    command.Parameters.AddWithValue("@ambientes", inmueble.Ambientes);
                    command.Parameters.AddWithValue("@superficie", inmueble.Superficie);
                    command.Parameters.AddWithValue("@latitud", inmueble.Latitud);
                    command.Parameters.AddWithValue("@longitud", inmueble.Longitud);
                    command.Parameters.AddWithValue("@precio", inmueble.Precio);
                    command.Parameters.AddWithValue("@propietarioId", inmueble.PropietarioId);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        public List<Inmueble> ObtenerTodos()
        {
            List<Inmueble> res = new List<Inmueble>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT inmueble.Id, Direccion, Ambientes, Superficie, Latitud, Longitud, Precio, inmueble.Estado, PropietarioId, " +
                    "propietario.Nombre, propietario.Apellido, propietario.Estado  " +
                    $"FROM Inmuebles inmueble INNER JOIN Propietarios propietario ON inmueble.PropietarioId = propietario.id " +
                    $"WHERE inmueble.Estado = 1 AND propietario.Estado = 1";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Inmueble inmueble = new Inmueble
                        {
                            Id = reader.GetInt32(0),
                            Direccion = reader.GetString(1),
                            Ambientes = reader.GetInt32(2),
                            Superficie = reader.GetInt32(3),
                            Latitud = reader.GetDecimal(4),
                            Longitud = reader.GetDecimal(5),
                            Precio = reader.GetDecimal(6),
                            Estado = reader.GetBoolean(7),
                            PropietarioId = reader.GetInt32(8),
                            Propietario = new Propietario
                            {
                                Id = reader.GetInt32(8),
                                Nombre = reader.GetString(9),
                                Apellido = reader.GetString(10),
                            }
                        };
                        res.Add(inmueble);
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public Inmueble ObtenerPorId(int id)
        {
            Inmueble inmueble = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT inmueble.id, Direccion, Ambientes, Superficie, Latitud, Longitud, Precio, PropietarioId, " +
                    "propietario.Nombre, propietario.Apellido " +
                    $"FROM Inmuebles inmueble INNER JOIN Propietarios propietario ON inmueble.PropietarioId = propietario.id " +
                    $"WHERE inmueble.id = @id AND inmueble.Estado = 1";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        inmueble = new Inmueble
                        {
                            Id = reader.GetInt32(0),
                            Direccion = reader.GetString(1),
                            Ambientes = reader.GetInt32(2),
                            Superficie = reader.GetInt32(3),
                            Latitud = reader.GetDecimal(4),
                            Longitud = reader.GetDecimal(5),
                            Precio = reader.GetDecimal(6),
                            PropietarioId = reader.GetInt32(7),
                            Propietario = new Propietario
                            {
                                Id = reader.GetInt32(7),
                                Nombre = reader.GetString(8),
                                Apellido = reader.GetString(9),
                            }
                        };
                    }
                    connection.Close();
                }
            }
            return inmueble;
        }

        public List<Inmueble> ObtenerPorPropietario(int id)
        {
            List<Inmueble> res = new List<Inmueble>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT inmueble.Id, Direccion, Ambientes, Superficie, Latitud, Longitud, Precio, PropietarioId, " +
                    "propietario.Nombre, propietario.Apellido " +
                    $" FROM Inmuebles inmueble INNER JOIN Propietarios propietario ON inmueble.PropietarioId = propietario.id " +
                    $"WHERE propietario.id = @id AND inmueble.Estado = 1";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Inmueble inmueble = new Inmueble
                        {
                            Id = reader.GetInt32(0),
                            Direccion = reader.GetString(1),
                            Ambientes = reader.GetInt32(2),
                            Superficie = reader.GetInt32(3),
                            Latitud = reader.GetDecimal(4),
                            Longitud = reader.GetDecimal(5),
                            Precio = reader.GetDecimal(6),
                            PropietarioId = reader.GetInt32(7),
                            Propietario = new Propietario
                            {
                                Id = reader.GetInt32(7),
                                Nombre = reader.GetString(8),
                                Apellido = reader.GetString(9),
                            }
                        };
                        res.Add(inmueble);
                    }
                    connection.Close();
                }
            }
            return res;
        }

        // Dadas dos fechas posibles de un contrato(inicio y fin), listar todos los inmuebles que no estén ocupados en algún contrato entre esas fechas.

        public List<Inmueble> ObtenerInmueblesPorFechas(DateTime fechaDesde, DateTime fechaHasta)
        {
            List<Inmueble> res = new List<Inmueble>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT inmueble.Id, Direccion, Ambientes, Superficie, Latitud, Longitud, Precio, inmueble.Estado, PropietarioId, " +
                    "propietario.Nombre, propietario.Apellido, propietario.Estado, contrato.FechaHasta, contrato.FechaDesde  " +
                    $"FROM Inmuebles inmueble INNER JOIN Propietarios propietario ON inmueble.PropietarioId = propietario.id " +
                    $"INNER JOIN Contratos contrato ON contrato.InmuebleId = inmueble.id " +
                    $"WHERE inmueble.Estado = 1 AND propietario.Estado = 1 AND inmueble.Id " +
                    $"IN " +
                    $"(SELECT InmuebleId  From Contratos WHERE(FechaDesde > @fechaHasta OR FechaHasta < @fechaDesde))";
                   
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@fechaDesde", fechaDesde);
                    command.Parameters.AddWithValue("@fechaHasta", fechaHasta);
                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Inmueble inmueble = new Inmueble
                        {
                            Id = reader.GetInt32(0),
                            Direccion = reader.GetString(1),
                            Ambientes = reader.GetInt32(2),
                            Superficie = reader.GetInt32(3),
                            Latitud = reader.GetDecimal(4),
                            Longitud = reader.GetDecimal(5),
                            Precio = reader.GetDecimal(6),
                            Estado = reader.GetBoolean(7),
                            PropietarioId = reader.GetInt32(8),
                            Propietario = new Propietario
                            {
                                Id = reader.GetInt32(8),
                                Nombre = reader.GetString(9),
                                Apellido = reader.GetString(10),
                            }
                        };
                        res.Add(inmueble);
                    }
                    connection.Close();
                }
            }
            return res;
        }
        


    }
}
