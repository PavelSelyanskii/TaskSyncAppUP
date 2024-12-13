﻿namespace TaskSyncApp.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; } // Статус задачи (например, "Новая", "В процессе", "Завершена")
    }
}