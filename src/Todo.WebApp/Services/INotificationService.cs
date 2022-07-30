﻿using Todo.WebApp.Models;

namespace Todo.WebApp.Services
{
    public interface INotificationService
    {
        void Start();
        void AddToast(string? id, string? type);

        event EventHandler<TodoTaskOutput> TaskCreated;
        event EventHandler<TodoTaskOutput> TaskUpdated;
        event EventHandler<TodoTaskOutput> TaskDeleted;
    }
}
