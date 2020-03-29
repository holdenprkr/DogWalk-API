using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DogWalkAPI.Models;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DogWalkAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OwnerController : Controller
    {
        private readonly IConfiguration _config;

        public OwnerController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, Name, Address, NeighborhoodId, Phone FROM Owner";
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Owner> owners = new List<Owner>();

                    while (reader.Read())
                    {
                        Owner owner = new Owner
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Address = reader.GetString(reader.GetOrdinal("Address")),
                            Phone = reader.GetString(reader.GetOrdinal("Phone")),
                            NeighborhoodId = reader.GetInt32(reader.GetOrdinal("NeighborhoodId"))
                        };

                        owners.Add(owner);
                    }
                    reader.Close();

                    return Ok(owners);
                }
            }
        }

        [HttpGet("{id}", Name = "GetOwners")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT o.Id, o.Name, o.Address, o.NeighborhoodId, o.Phone, d.id AS DogId, d.Name AS DogName, d.Breed, d.OwnerId
                        FROM Owner o
                        LEFT JOIN Dog d ON o.Id = d.OwnerId
                        WHERE o.Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Owner owner = null;

                    while (reader.Read())
                    {
                        if (owner == null)
                        {
                            owner = new Owner
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Address = reader.GetString(reader.GetOrdinal("Address")),
                                Phone = reader.GetString(reader.GetOrdinal("Phone")),
                                NeighborhoodId = reader.GetInt32(reader.GetOrdinal("NeighborhoodId")),
                                Dogs = new List<Dog>()
                            };
                        }

                        owner.Dogs.Add(new Dog() 
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("DogId")),
                            Name = reader.GetString(reader.GetOrdinal("DogName")),
                            Breed = reader.GetString(reader.GetOrdinal("Breed")),
                            OwnerId = reader.GetInt32(reader.GetOrdinal("OwnerId"))
                        });
                    }
                    reader.Close();

                    return Ok(owner);
                }
            }
        }

        //[HttpPost]
        //public async Task<IActionResult> Post([FromBody] Dog dog)
        //{
        //    using (SqlConnection conn = Connection)
        //    {
        //        conn.Open();
        //        using (SqlCommand cmd = conn.CreateCommand())
        //        {
        //            cmd.CommandText = @"INSERT INTO Dog (Name, Breed, OwnerId)
        //                                OUTPUT INSERTED.Id
        //                                VALUES (@name, @breed, @ownerId)";
        //            cmd.Parameters.Add(new SqlParameter("@name", dog.Name));
        //            cmd.Parameters.Add(new SqlParameter("@breed", dog.Breed));
        //            cmd.Parameters.Add(new SqlParameter("@ownerId", dog.OwnerId));


        //            int newId = (int)cmd.ExecuteScalar();
        //            dog.Id = newId;
        //            return CreatedAtRoute("GetDogs", new { id = newId }, dog);
        //        }
        //    }
        //}

        //[HttpPut("{id}")]
        //public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Dog dog)
        //{
        //    try
        //    {
        //        using (SqlConnection conn = Connection)
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd = conn.CreateCommand())
        //            {
        //                cmd.CommandText = @"UPDATE Dog
        //                                    SET Name = @name,
        //                                        Breed = @breed,
        //                                        OwnerId = @ownerId
        //                                    WHERE Id = @id";
        //                cmd.Parameters.Add(new SqlParameter("@name", dog.Name));
        //                cmd.Parameters.Add(new SqlParameter("@breed", dog.Breed));
        //                cmd.Parameters.Add(new SqlParameter("@ownerId", dog.OwnerId));
        //                cmd.Parameters.Add(new SqlParameter("@id", id));

        //                int rowsAffected = cmd.ExecuteNonQuery();
        //                if (rowsAffected > 0)
        //                {
        //                    return new StatusCodeResult(StatusCodes.Status204NoContent);
        //                }
        //                throw new Exception("No rows affected");
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        if (!DogExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    private bool DogExists(int id)
        //    {
        //        using (SqlConnection conn = Connection)
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd = conn.CreateCommand())
        //            {
        //                cmd.CommandText = @"
        //                SELECT Id, Name, Breed, OwnerId
        //                FROM Dog
        //                WHERE Id = @id";
        //                cmd.Parameters.Add(new SqlParameter("@id", id));

        //                SqlDataReader reader = cmd.ExecuteReader();
        //                return reader.Read();
        //            }
        //        }
        //    }
        //}
    }
}
