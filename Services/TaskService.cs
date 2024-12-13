using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TaskSyncApp.Models;

namespace TaskSyncApp.Services
{
    public class TaskService
    {
        private readonly SQLiteAsyncConnection _database;

        public TaskService(string dbPath)
        {
            try
            {
                // Проверка директории для базы данных
                var directory = Path.GetDirectoryName(dbPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                    Console.WriteLine("Директория для базы данных создана.");
                }

                // Создаем подключение к базе данных
                _database = new SQLiteAsyncConnection(dbPath);
                Console.WriteLine($"Подключение к базе данных: {dbPath}");

                // Создаем таблицу, если она еще не существует
                _database.CreateTableAsync<TaskItem>().Wait();
                Console.WriteLine("Таблица TaskItem создана или уже существует.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при инициализации базы данных: {ex.Message}");
                _database = null;
            }
        }

        // Получение всех задач из базы данных с обработкой ошибок
        public async Task<List<TaskItem>> GetTasksAsync()
        {
            if (_database == null)
            {
                Console.WriteLine("Ошибка: база данных не инициализирована.");
                return new List<TaskItem>(); // Возвращаем пустой список, если база данных не инициализирована
            }

            try
            {
                var tasks = await _database.Table<TaskItem>().ToListAsync();
                Console.WriteLine($"Задачи получены: {tasks.Count}");
                return tasks;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении задач: {ex.Message}");
                return new List<TaskItem>(); // Возвращаем пустой список в случае ошибки
            }
        }

        // Сохранение задачи в базе данных
        public async Task<int> SaveTaskAsync(TaskItem task)
        {
            if (_database == null)
            {
                Console.WriteLine("Ошибка: база данных не инициализирована.");
                return 0;
            }

            try
            {
                if (task.Id != 0)
                {
                    return await _database.UpdateAsync(task); // Обновление задачи
                }
                else
                {
                    return await _database.InsertAsync(task); // Добавление новой задачи
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении задачи: {ex.Message}");
                return 0;
            }
        }

        // Удаление задачи из базы данных
        public async Task<int> DeleteTaskAsync(TaskItem task)
        {
            if (_database == null)
            {
                Console.WriteLine("Ошибка: база данных не инициализирована.");
                return 0;
            }

            try
            {
                return await _database.DeleteAsync(task);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при удалении задачи: {ex.Message}");
                return 0;
            }
        }

        // Очистка всех задач в базе данных
        public async Task<int> ClearTasksAsync()
        {
            if (_database == null)
            {
                Console.WriteLine("Ошибка: база данных не инициализирована.");
                return 0;
            }

            try
            {
                return await _database.DeleteAllAsync<TaskItem>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при очистке задач: {ex.Message}");
                return 0;
            }
        }
    }
}
