using System.Collections.Generic;
using System.Threading.Tasks;
using Tasklify.Contracts;
using Tasklify.Interfaces;

namespace Tasklify.Repositories
{
    public class DemoTaskRepository : TaskRepository
    {
        private readonly ITasksDAL _tDal;

        public DemoTaskRepository(ITasksDAL tasksDAL)
        {
            _tDal = tasksDAL;
        }

        public async Task<TasklifyTask> AddAsync(string summary, string description, int assignee)
        {
            return await _tDal.AddAsync(summary, description, assignee);
        }

        public async Task<IList<TasklifyTask>> GetAllAsync()
        {
            return await _tDal.GetAllAsync();
        }

        public async Task<TasklifyTask> GetByIdAsync(int id)
        {
            return await _tDal.GetByIdAsync(id);
        }

        public async Task<bool> RemoveByIdAsync(int id)
        {
            return await _tDal.RemoveByIdAsync(id);
        }

        public async Task<TasklifyTask> UpdateByIdAsync(int id, TasklifyTask task)
        {
            return await _tDal.UpdateByIdAsync(id, task);
        }
    }
}
