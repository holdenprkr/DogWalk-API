using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DogWalkAPI.Models
{
    public class Owner
    {
        public int Id { get; set; }

        [Required]
        [StringLength(40, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public int NeighborhoodId { get; set; }

        public string Phone { get; set; }

        public List<Dog> Dogs { get; set; }

        public Neighborhood Neighborhood { get; set; }
    }
}