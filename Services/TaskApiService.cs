using TaskSyncApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TaskSyncApp.Services
{
    public class TaskApiService
    {
        private readonly ApiClient _apiClient;

        public TaskApiService()
        {
            _apiClient = new ApiClient();
        }

        // Получение всех задач с сервера
        public async Task<List<TaskItem>> GetTasksFromApiAsync()
        {
            return await _apiClient.GetAsync<TaskItem>("tasks");
        }

        // Отправка задачи на сервер
        public async Task<bool> SyncTaskToApiAsync(TaskItem task)
        {
            return await _apiClient.PostAsync("tasks", task);
        }

        // Обновление задачи на сервере
        public async Task<bool> UpdateTaskOnApiAsync(TaskItem task)
        {
            return await _apiClient.PutAsync("tasks", task.Id, task);
        }

        // Удаление задачи с сервера
        public async Task<bool> DeleteTaskFromApiAsync(int taskId)
        {
            return await _apiClient.DeleteAsync("tasks", taskId);
        }
    }
}
