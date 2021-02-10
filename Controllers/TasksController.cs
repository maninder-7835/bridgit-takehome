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
    public class TasksController : ControllerBase
    {
        private readonly IUsersDAL _uDal;
        private readonly ITasksDAL _tDal;
        
        public TasksController(IUsersDAL userDal, ITasksDAL taskDal)
        {
            _uDal = userDal;
            _tDal = taskDal;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _tDal.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                return Ok(await _tDal.GetByIdAsync(id));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] TasklifyTask task)
        {
            try
            {
                // Validating the task before calling the AddAsync method
                ValidateTask(task);

                var tmpTask = await _tDal.AddAsync(task.Summary, task.Description);                
                return Ok(tmpTask);
            }
            catch(FormatException fex)
            {
                string errorMessage = string.Format("Error while creating new task with \r\nSummary:{0} \r\nDescription {1}. \r\nException:{2}", task.Summary, task.Description, fex.Message);
                return StatusCode(StatusCodes.Status400BadRequest, errorMessage);
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Error while creating new task with \r\nSummary:{0} \r\nDescription {1}. \r\nException:{2}", task.Summary, task.Description, ex.Message);
                return StatusCode(StatusCodes.Status503ServiceUnavailable, errorMessage);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateById(int id, [FromBody] TasklifyTask task)
        {
            try
            {
                // Validating the task before calling the UpdateByIdAsync method
                ValidateTask(task);
                return Ok(await _tDal.UpdateByIdAsync(id, task));
            }
            catch (FormatException fex)
            {
                string errorMessage = string.Format("Error while updating task with \r\nid:{0}. \r\nException:{1}", id, fex.Message);
                return StatusCode(StatusCodes.Status400BadRequest, errorMessage);
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("Error while updating task with \r\nid:{0}. \r\nException:{1}", id, ex.Message);
                return StatusCode(StatusCodes.Status503ServiceUnavailable, errorMessage);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteById(int id)
        {
            await _tDal.RemoveByIdAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Validation method for the task object
        /// </summary>
        /// <param name="task"></param>
        private void ValidateTask(TasklifyTask task)
        {
            string errorMessage = "";
            if (string.IsNullOrWhiteSpace(task.Summary))
                errorMessage = "Task Summary cannot be empty.";
            else if (task.Summary.Length > 100)
                errorMessage = "Task Summary should be 100 characters or less.";

            if (!string.IsNullOrWhiteSpace(task.Description) && task.Description.Length > 500)
                errorMessage += " Task Description should be 500 characters or less.";

            if (!string.IsNullOrEmpty(errorMessage))
                throw new FormatException(errorMessage);
        }
    }
}
