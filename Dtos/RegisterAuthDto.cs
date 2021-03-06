using System;
using System.ComponentModel.DataAnnotations;

namespace Amingo.Dtos
{
	public class RegisterAuthDto
	{
		[Required]
		public string Username { get; set; }

		[Required]
		[MinLength(4, ErrorMessage = "Password too short")]
		public string Password { get; set; }

		[Required]
		public string Gender { get; set; }

		[Required]
		public DateTime DateOfBirth { get; set; }

		[Required]
		public string KnowAs { get; set; }

		[Required]
		public string City { get; set; }

		[Required]
		public string Country { get; set; }
		public DateTime LastActive { get; set; }
		public DateTime Created_at { get; set; }

		public RegisterAuthDto()
		{
			Created_at = DateTime.Now;
			LastActive = DateTime.Now;
		}
	}
}