using System;
using System.ComponentModel.DataAnnotations;

namespace DogWalkAPI.Models
{
	public class Dog
	{
			public int Id { get; set; }
			
			[Required]
			[StringLength(40, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 2)]

			public string Name { get; set; }

			[Required]
			public int OwnerId { get; set; }

			public Owner Owner { get; set; }
		
			public string Breed { get; set; }

			public string Notes { get; set; }

	}
}
