using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
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

                var tmpTask = await _tDal.AddAsync(task.Summary, task.Description, task.Assignee);                
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

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateById(int id, [FromBody] JsonPatchDocument<TasklifyTask> patch)
        {
            try
            {
                var entity = await _tDal.GetByIdAsync(id);
                if (entity == null)
                    return NotFound();

                ValidatePatchForValidAssignee(patch);

                patch.ApplyTo(entity, ModelState);

                return Ok(entity);

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
            
            if (!IsValidAssignee(task.Assignee).Result)
                errorMessage += " Task Assignee is not valid User.";

            if (!string.IsNullOrEmpty(errorMessage))
                throw new FormatException(errorMessage);
        }

        /// <summary>
        /// Validating the assigned ID with list of Users
        /// </summary>
        /// <param name="assignee"></param>
        /// <returns></returns>
        private async Task<bool> IsValidAssignee(int assignee)
        {
            if (assignee != 0)
            {
                var users = await _uDal.GetUsersAsync();
                return users.Where(a => a.Id == assignee).FirstOrDefault() != null ? true : false;                
            }
            else
                return true;
        }

        /// <summary>
        /// Validating Assignee ID for the Patch
        /// </summary>
        /// <param name="patch"></param>
        private void ValidatePatchForValidAssignee(JsonPatchDocument<TasklifyTask> patch)
        {
            foreach (var operation in patch.Operations)
            {
                if (operation.path.ToLower() == "/assignee_id" && operation.op.ToLower() == "replace")
                {
                    if (!IsValidAssignee(Convert.ToInt32(operation.value)).Result)
                        throw new FormatException("Task Assignee is not valid User");
                }
            }

        }
    }
}
