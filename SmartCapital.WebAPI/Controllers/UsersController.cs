using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCapital.WebAPI.Application.Exceptions;
using SmartCapital.WebAPI.Application.Interfaces;
using SmartCapital.WebAPI.Domain.Domain;
using SmartCapital.WebAPI.DTO.AddRequests;
using SmartCapital.WebAPI.DTO.Responses;
using SmartCapital.WebAPI.DTO.UpdateRequests;
using SmartCapital.WebAPI.Models;

namespace SmartCapital.WebAPI.Controllers;

/// <summary>
/// Controller responsible for managing operations related to users.
/// </summary>
[Route("api/")]
[ApiController]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    /// <summary>
    /// Initializes a new instance of <see cref="UsersController"/> with the provided user service.
    /// </summary>
    /// <param name="userService">Service to manage user operations.</param>
    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Gets the user corresponding to the provided name.
    /// </summary>
    /// <param name="userName">Name of the user to be retrieved.</param>
    /// <returns>A <see cref="UserResponse"/> object representing the found user.</returns>
    /// <response code="200">User successfully found.</response>
    /// <response code="404">User with the provided name not found.</response>
    /// <response code="403">Access to the resource is forbidden.</response>
    [HttpGet("{userName}")]
    [ProducesResponseType(typeof(UserResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> GetUserByName([FromRoute] string userName)
    {
        var user = await _userService.GetUserByNameAsync(userName);

        var userNameFromToken = HttpContext.Items["User"] as string;

        if (user != null)
        {
            if (userNameFromToken == userName)
            {
                return Ok(user.ToUserResponse());
            }

            return Forbid();
        }

        return NotFound(new ErrorResponse
        {
            ErrorType = "UserNotFound",
            Message = "The user with the provided name was not found."
        });
    }

    /// <summary>
    /// Creates a new user in the system.
    /// </summary>
    /// <param name="user">Data of the user to be created.</param>
    /// <returns>The newly created user.</returns>
    /// <response code="201">User successfully created.</response>
    /// <response code="400">Error creating the user, the request body is empty.</response>
    /// <response code="422">Error creating the user due to validation or duplication issues.</response>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(UserResponse), 201)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 422)]
    public async Task<IActionResult> AddUser([FromBody] UserAddRequest user)
    {
        if (user == null)
            return BadRequest(new ErrorResponse()
            {
                ErrorType = "EmptyUserAddRequest",
                Message = "The user addition request cannot be null."
            });

        try
        {
            await _userService.AddUserAsync(user.ToUser());
        }
        catch (ArgumentException ex)
        {
            return UnprocessableEntity(new ErrorResponse
            {
                ErrorType = "ValidationError",
                Message = $"Validation error: {ex.Message}"
            });
        }
        catch (ExistingUserException ex)
        {
            return UnprocessableEntity(new ErrorResponse
            {
                ErrorType = "UserCreationError",
                Message = $"Error creating the user: {ex.Message}"
            });
        }

        return CreatedAtRoute("", new { user.UserName });
    }

    /// <summary>
    /// Deletes the user corresponding to the provided name.
    /// </summary>
    /// <param name="userName">Name of the user to be deleted.</param>
    /// <returns>Status indicating the result of the operation.</returns>
    /// <response code="204">User successfully deleted.</response>
    /// <response code="403">Access to the resource is forbidden.</response>
    /// <response code="404">User with the provided name not found.</response>
    [HttpDelete("{userName}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(403)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    public async Task<IActionResult> DeleteUser([FromRoute] string userName)
    {
        var userToRemove = await _userService.GetUserByNameAsync(userName);

        var userNameFromToken = HttpContext.Items["User"] as string;

        if (userNameFromToken != userName)
        {
            return Forbid();
        }

        if (userToRemove == null)
            return NotFound(new ErrorResponse
            {
                ErrorType = "UserNotFound",
                Message = "The user with the provided name was not found."
            });

        await _userService.RemoveUserAsync(userToRemove);

        return NoContent();
    }

    /// <summary>
    /// Updates an existing user based on the provided name.
    /// </summary>
    /// <param name="userName">Name of the user to be updated.</param>
    /// <param name="user">Updated user data.</param>
    /// <returns>Status indicating the result of the operation.</returns>
    /// <response code="204">User successfully updated.</response>
    /// <response code="403">Access to the resource is forbidden.</response>
    /// <response code="404">User with the provided name not found.</response>
    /// <response code="400">Error updating the user, the request body is empty.</response>
    /// <response code="422">Error updating the user due to validation or duplication issues.</response>
    [HttpPut("{userName}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 422)]
    public async Task<IActionResult> UpdateUser([FromRoute] string userName, [FromBody] UserUpdateRequest user)
    {
        if (user == null)
            return BadRequest(new ErrorResponse()
            {
                ErrorType = "EmptyUserUpdateRequest",
                Message = "The user update request cannot be null."
            });

        User? updatedUser;

        var userNameFromToken = HttpContext.Items["User"] as string;
        var role = HttpContext.Items["Role"] as string;

        try
        {
            if (userNameFromToken != userName && role != "Admin")
            {
                return Forbid();
            }

            updatedUser = await _userService.UpdateUserAsync(userName, user.ToUser());
        }
        catch (ArgumentException ex)
        {
            return UnprocessableEntity(new ErrorResponse
            {
                ErrorType = "ValidationError",
                Message = $"Validation error: {ex.Message}"
            });
        }
        catch (ExistingUserException ex)
        {
            return UnprocessableEntity(new ErrorResponse
            {
                ErrorType = "UserUpdateError",
                Message = $"Error updating the user: {ex.Message}"
            });
        }

        if (updatedUser == null)
            return NotFound(new ErrorResponse
            {
                ErrorType = "UserNotFound",
                Message = "The user with the provided name was not found."
            });

        return NoContent();
    }
}
