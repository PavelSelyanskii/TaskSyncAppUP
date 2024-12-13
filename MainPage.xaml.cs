using TaskSyncApp.Models;
using TaskSyncApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace TaskSyncApp
{
    public partial class MainPage : ContentPage
    {
        private readonly TaskService _taskService;
        private readonly TaskApiService _taskApiService;

        // ObservableCollection для привязки к UI
        public ObservableCollection<TaskItem> Tasks { get; set; }

        public MainPage()
        {
            InitializeComponent();
            _taskService = new TaskService("tasks.db");
            _taskApiService = new TaskApiService();

            // Инициализация ObservableCollection
            Tasks = new ObservableCollection<TaskItem>();

            // Привязываем источник данных к CollectionView
            TaskCollectionView.ItemsSource = Tasks;
            BindingContext = this;
        }

        // Загрузка задач при появлении страницы
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Получение задач из базы данных
            var tasksFromDb = await _taskService.GetTasksAsync();

            // Очистка текущей коллекции и добавление задач
            Tasks.Clear();
            foreach (var task in tasksFromDb)
            {
                Tasks.Add(task);
            }
        }

        // Добавление новой задачи
        private async void OnAddTaskClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TaskTitleEntry.Text))
            {
                await DisplayAlert("Ошибка", "Введите название задачи.", "OK");
                return;
            }

            var newTask = new TaskItem
            {
                Title = TaskTitleEntry.Text,
                Description = TaskDescriptionEntry.Text,
                DueDate = TaskDueDatePicker.Date,
                Status = TaskStatusPicker.SelectedItem?.ToString() ?? "Новая"
            };

            // Сохраняем задачу в базу данных
            await _taskService.SaveTaskAsync(newTask);

            // Синхронизация с сервером
            await _taskApiService.SyncTaskToApiAsync(newTask);

            // Добавляем задачу в ObservableCollection
            Tasks.Add(newTask);

            // Сбрасываем поля ввода
            TaskTitleEntry.Text = string.Empty;
            TaskDescriptionEntry.Text = string.Empty;
            TaskDueDatePicker.Date = DateTime.Now;
            TaskStatusPicker.SelectedIndex = -1;

            await DisplayAlert("Успех", "Задача добавлена.", "OK");
        }

        // Применение фильтра к списку задач
        private async void OnFilterClicked(object sender, EventArgs e)
        {
            var selectedStatus = TaskStatusPicker.SelectedItem?.ToString();
            var selectedDate = FilterDatePicker.Date;

            // Получаем задачи из базы данных
            var tasksFromDb = await _taskService.GetTasksAsync();

            // Применяем фильтры
            if (!string.IsNullOrEmpty(selectedStatus))
            {
                tasksFromDb = tasksFromDb.Where(t => t.Status == selectedStatus).ToList();
            }

            if (FilterDatePicker.IsEnabled) // Проверяем, включен ли фильтр по дате
            {
                tasksFromDb = tasksFromDb.Where(t => t.DueDate.Date == selectedDate.Date).ToList();
            }

            if (tasksFromDb.Count == 0)
            {
                await DisplayAlert("Результаты", "Нет задач, соответствующих выбранным фильтрам.", "OK");
            }

            // Обновляем ObservableCollection
            Tasks.Clear();
            foreach (var task in tasksFromDb)
            {
                Tasks.Add(task);
            }
        }


        // Удаление задачи
        private async void OnDeleteTaskClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var task = (TaskItem)button.BindingContext;

            var confirm = await DisplayAlert("Подтверждение", "Вы уверены, что хотите удалить эту задачу?", "Да", "Нет");
            if (!confirm)
                return;

            // Удаляем задачу из базы данных
            await _taskService.DeleteTaskAsync(task);

            // Удаляем задачу из сервера (если необходимо)
            await _taskApiService.DeleteTaskFromApiAsync(task.Id);

            // Удаляем задачу из коллекции
            Tasks.Remove(task);
        }

        // Редактирование задачи
        private async void OnEditTaskClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var task = (TaskItem)button.BindingContext;

            // Здесь можно реализовать открытие окна редактирования задачи
            await DisplayAlert("Редактирование задачи", $"Редактирование задачи: {task.Title}", "OK");
        }

        // Синхронизация задач
        private async void OnSyncClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Синхронизация", "Синхронизация задач...", "OK");

            // Получение задач с сервера
            var tasksFromServer = await _taskApiService.GetTasksFromApiAsync();

            // Сохранение задач в базу данных и обновление списка
            foreach (var task in tasksFromServer)
            {
                await _taskService.SaveTaskAsync(task);

                if (!Tasks.Any(t => t.Id == task.Id))
                {
                    Tasks.Add(task);
                }
            }
        }
    }
}
