using System.Collections.Generic;
using System.Threading.Tasks;
using Tasklify.Contracts;

namespace Tasklify.Interfaces
{
    public interface TaskRepository
    {
        Task<IList<TasklifyTask>> GetAllAsync();

        Task<TasklifyTask> GetByIdAsync(int id);

        Task<TasklifyTask> UpdateByIdAsync(int id, TasklifyTask task);

        Task<bool> RemoveByIdAsync(int id);

        Task<TasklifyTask> AddAsync(string summary, string description, int assignee);
    }
}
