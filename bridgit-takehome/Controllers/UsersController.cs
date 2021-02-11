using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Tasklify.Contracts;
using Tasklify.Interfaces;

namespace Tasklify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersDAL _dal;

        public UsersController(IUsersDAL userDal)
        {
            _dal = userDal;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _dal.GetUsersAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                return Ok(await _dal.GetUserByIdAsync(id));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
        }

        [Route("[action]/{email}")]
        [HttpGet]
        public async Task<IActionResult> GetByEmail(string email)
        {
            try
            {
                return Ok(await _dal.GetUserByEmailAsync(email));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] TasklifyUser user)
        {
            try
            {
                ValidateUser(user);
                var tmpUser = await _dal.AddUserAsync(user.Email, user.Name);
                return Ok(tmpUser);
            }
            catch (FormatException fex)
            {
                string errorMessage = string.Format("Error while adding the User with \r\nEmail:{0}. \r\nName:{1} \r\nException:{2}", user.Email, user.Name, fex.Message);
                return StatusCode(StatusCodes.Status400BadRequest, errorMessage);
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Error while adding the User with \r\nEmail:{0}. \r\nName:{1} \r\nException:{2}", user.Email, user.Name, ex.Message);
                return StatusCode(StatusCodes.Status503ServiceUnavailable, errorMessage);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateById(int id, [FromBody] TasklifyUser user)
        {
            try
            {
                ValidateUser(user);
                var result = await _dal.UpdateUserByIdAsync(id, user);
                if (result)
                    return Ok(await _dal.GetUserByIdAsync(id));
                return NoContent();
            }
            catch (FormatException fex)
            {
                string errorMessage = string.Format("Error while updating user with \r\nid:{0}. \r\nException:{1}", id, fex.Message);
                return StatusCode(StatusCodes.Status400BadRequest, errorMessage);
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Error while updating user with \r\nid:{0}. \r\nException:{1}", id, ex.Message);
                return StatusCode(StatusCodes.Status503ServiceUnavailable, errorMessage);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteById(int id)
        {
            await _dal.RemoveUserByIdAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Validating the user object for email and name
        /// </summary>
        /// <param name="user"></param>
        private void ValidateUser(TasklifyUser user)
        {
            string errorMessage = "";
            if (string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrWhiteSpace(user.Name))
                errorMessage = "User email and/or name cannot be empty.";

            if (!string.IsNullOrWhiteSpace(user.Email) && user.Email.Length > 100)
                errorMessage = "User email should be 100 characters or less.";

            if (!string.IsNullOrWhiteSpace(user.Name) && user.Name.Length > 150)
                errorMessage += " User name should be 150 characters or less.";

            if (!string.IsNullOrEmpty(errorMessage))
                throw new FormatException(errorMessage);
        }
    }
}
