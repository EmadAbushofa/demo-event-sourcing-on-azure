﻿using Microsoft.EntityFrameworkCore;
using Todo.Query.Abstractions;
using Todo.Query.Entities;
using Todo.Query.QueryHandlers.Filter;

namespace Todo.Query.Infrastructure.Data
{
    public class TasksRepository : ITasksRepository
    {
        private readonly TodoTasksDbContext _context;

        public TasksRepository(TodoTasksDbContext context)
        {
            _context = context;
        }

        public async Task<FilterResult> FilterAsync(FilterQuery filter, CancellationToken cancellationToken)
        {
            var query = _context.Tasks.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.UserId))
                query = query.Where(t => t.UserId == filter.UserId);

            if (filter.IsCompleted != null)
                query = query.Where(t => t.IsCompleted == filter.IsCompleted);

            if (filter.DueDateFrom != null)
                query = query.Where(t => t.DueDate >= filter.DueDateFrom);

            if (filter.DueDateTo != null)
                query = query.Where(t => t.DueDate <= filter.DueDateTo);

            var total = await query.CountAsync(cancellationToken);

            var results = await query.Skip(filter.Skip)
                .Take(filter.Size)
                .OrderBy(t => t.ClusterIndex)
                .ToListAsync(cancellationToken);

            return new FilterResult(
                Page: filter.Page,
                Size: filter.Size,
                Total: total,
                Tasks: results
            );
        }

        public Task<TodoTask?> FindAsync(Guid id, CancellationToken cancellationToken) =>
            _context.Tasks.FindAsync(new object[] { id }, cancellationToken: cancellationToken).AsTask();

        public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken) =>
            _context.Tasks.AnyAsync(t => t.Id == id, cancellationToken);

        public Task<bool> HasSimilarTodoTaskAsync(string userId, string title, CancellationToken cancellationToken)
        {
            title = title.Trim().ToUpper();

            return _context.Tasks.AnyAsync(
                            t =>
                                t.UserId == userId &&
                                t.NormalizedTitle == title &&
                                t.IsCompleted == false
                        , cancellationToken);
        }

        public Task<TodoTask?> GetSimilarTodoTaskAsync(string userId, string title, Guid? excludedId, CancellationToken cancellationToken)
        {
            title = title.Trim().ToUpper();

            if (excludedId == null)
                return _context.Tasks.FirstOrDefaultAsync(
                                t =>
                                    t.UserId == userId &&
                                    t.NormalizedTitle == title &&
                                    t.IsCompleted == false
                            , cancellationToken);

            return _context.Tasks.FirstOrDefaultAsync(
                            t =>
                                t.UserId == userId &&
                                t.NormalizedTitle == title &&
                                t.Id != excludedId &&
                                t.IsCompleted == false
                        , cancellationToken);
        }

        public Task AddAsync(TodoTask task) => _context.Tasks.AddAsync(task).AsTask();
        public Task RemoveAsync(TodoTask task)
        {
            _context.Tasks.Remove(task);
            return Task.CompletedTask;
        }
    }
}
