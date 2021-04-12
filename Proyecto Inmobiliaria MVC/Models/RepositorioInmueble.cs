﻿using Microsoft.Extensions.Configuration;
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
            var res = 1;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"INSERT INTO Inmuebles (Direccion, Ambientes, Superficie, Latitud, Longitud, PropietarioId) " +
                    $"VALUES (@direccion, @ambientes, @superficie, @latitud, @longitud, @propietarioId);" +
                    "SELECT SCOPE_IDENTITY();"; // devuelve el id insertado (LAST_INSERT_ID para mysql)

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@direccion", inmueble.Direccion);
                    command.Parameters.AddWithValue("@ambientes", inmueble.Ambientes);
                    command.Parameters.AddWithValue("@superficie", inmueble.Superficie);
                    command.Parameters.AddWithValue("@latitud", inmueble.Longitud);
                    command.Parameters.AddWithValue("@longitud", inmueble.Longitud);
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
            var res = 1;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "DELETE FROM Inmuebles WHERE id = @id";

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
            var res = 1;

            using(SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"UPDATE Inmuebles SET Direccion = @direccion, Ambientes = @ambientes, " +
                    $"superficie = @superficie, Latitud = @latitud, Longitud = @longitud, PropietarioId = @propietarioId" +
                    $"WHERE id = @id";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@direccion", inmueble.Direccion);
                    command.Parameters.AddWithValue("@ambientes", inmueble.Ambientes);
                    command.Parameters.AddWithValue("@superficie", inmueble.Superficie);
                    command.Parameters.AddWithValue("@latitud", inmueble.Latitud);
                    command.Parameters.AddWithValue("@longitud", inmueble.Longitud);
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
                string sql = $"SELECT inmueble.Id, Direccion, Ambientes, Superficie, Latitud, Longitud, PropietarioId, " +
                    "propietario.Nombre, propietario.Apellido" +
                    $" FROM Inmuebles inmueble INNER JOIN Propietarios propietario ON inmueble.PropietarioId = propietario.id";

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
                            PropietarioId = reader.GetInt32(6),
                            Propietario = new Propietario
                            {
                                Id = reader.GetInt32(6),
                                Nombre = reader.GetString(7),
                                Apellido = reader.GetString(8),
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
            var contrato = new Inmueble();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT id, Direccion, Ambientes, Superficie, Latitud, Longitud, PropietarioId" +
                    "propietario.Nombre, propietario.Apellido" +
                    $" FROM Inmuebles inmuebles INNER JOIN Propietarios propietarios ON inmuebles.PropietarioId = propietario.id " +
                    $"WHERE inmuebles.id = @id;";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        Inmueble inmueble = new Inmueble
                        {
                            Id = reader.GetInt32(0),
                            Direccion = reader.GetString(1),
                            Ambientes = reader.GetInt32(2),
                            Superficie = reader.GetInt32(3),
                            Latitud = reader.GetDecimal(4),
                            Longitud = reader.GetDecimal(5),
                            PropietarioId = reader.GetInt32(6),
                            Propietario = new Propietario
                            {
                                Id = reader.GetInt32(6),
                                Nombre = reader.GetString(7),
                                Apellido = reader.GetString(8),
                            }
                        };
                    }
                    connection.Close();
                }
            }
            return contrato;
        }


    }
}
