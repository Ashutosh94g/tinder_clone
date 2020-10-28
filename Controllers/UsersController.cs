using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Amingo.Data;
using Amingo.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using System.Security.Claims;
using Amingo.Helpers;

namespace Amingo.Controller
{
	[ServiceFilter(typeof(LogUserActivity))]
	[Authorize]
	[ApiController]
	[Route("api/[controller]")]
	public class UsersController : ControllerBase
	{
		private readonly IAmingoRepo _repo;
		private readonly IMapper _mapper;

		public UsersController(IAmingoRepo repo, IMapper mapper)
		{
			_repo = repo;
			_mapper = mapper;
		}

		[HttpGet]
		//****************************************from query is used for telling the http to use initial values from query string
		public async Task<IActionResult> GetUsers([FromQuery] UserParams userParams)
		{
			var users = await _repo.GetUsers(userParams);
			var usersToReturn = _mapper.Map<IEnumerable<UserListDto>>(users);

			//adding pagination information to the response header
			//since we are in apiController we have access to Response
			Response.AddPagination(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

			return Ok(usersToReturn);
		}

		// [AllowAnonymous]
		[HttpGet("{id}", Name = nameof(GetUser))]
		public async Task<IActionResult> GetUser(int id)
		{
			var user = await _repo.GetUser(id);
			var userToReturn = _mapper.Map<UserDetailedDto>(user);
			return Ok(userToReturn);
		}

		[HttpPatch("{id}")]
		public async Task<IActionResult> UpdateUser(int id, JsonPatchDocument<UserUpdateDto> userPatchUpdateDto)
		{
			if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
				return Unauthorized();
			var user = await _repo.GetUser(id);
			var userToPatch = _mapper.Map<UserUpdateDto>(user);

			userPatchUpdateDto.ApplyTo(userToPatch, ModelState);
			if (!TryValidateModel(userToPatch))
			{
				ValidationProblem(ModelState);
			}

			_mapper.Map(userToPatch, user);
			if (await _repo.SaveAll())
			{
				return NoContent();
			}
			return BadRequest("Fields cannot be updated");
		}
	}
}