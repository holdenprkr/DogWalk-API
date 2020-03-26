using System;

namespace DogWalkAPI.Models
{
    public class Owner
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public int NeighborhoodId { get; set; }

        public string Phone { get; set; }
    }
}