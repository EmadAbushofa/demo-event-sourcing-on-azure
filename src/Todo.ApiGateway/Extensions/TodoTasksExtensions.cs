﻿using System.Security.Claims;
using Todo.ApiGateway.Models.TodoTasks;
using Todo.ApiGateway.TodoProto.Command;
using Todo.ApiGateway.TodoProto.Query;

namespace Todo.ApiGateway.Extensions
{
    public static class TodoTasksExtensions
    {
        public static FilterResult ToOutput(this FilterResponse response)
            => new()
            {
                Page = response.Page,
                Size = response.Size,
                Total = response.Total,
                Tasks = response.Tasks.Select(t => new TodoTaskFilterOutput()
                {
                    Id = t.Id,
                    Title = t.Title,
                    IsCompleted = t.IsCompleted,
                    DueDate = t.DueDate.ToDateTime(),
                    DuplicateTitle = t.DuplicateTitle,
                }).ToList()
            };

        public static TodoTaskOutput ToOutput(this FindResponse response)
            => new()
            {
                Id = response.Id,
                DueDate = response.DueDate.ToDateTime(),
                Note = response.Note,
                Title = response.Title,
                IsCompleted = response.IsCompleted,
                DuplicateTitle = response.DuplicateTitle,
            };

        public static SimilarTitleOutput ToOutput(this SimilarTitleExistsResponse response)
            => new()
            {
                Id = response.Id,
                Exists = response.Exists,
            };

        public static FilterRequest ToRequest(this FilterModel filter, ClaimsPrincipal claims)
            => new()
            {
                UserId = claims.GetId(),
                Page = filter.Page,
                Size = filter.Size,
                IsCompleted = filter.IsCompleted,
                DueDateTo = filter.DueDateTo?.DateToTimestamp(),
                DueDateFrom = filter.DueDateFrom?.DateToTimestamp(),
            };

        public static SimilarTitleExistsRequest ToRequest(this SimilarTitleQuery query, ClaimsPrincipal claims)
            => new()
            {
                UserId = claims.GetId(),
                Title = query.Title ?? "",
                ExcludedId = query.ExcludedId?.ToString(),
            };

        public static CreateRequest ToRequest(this CreateTaskInput input, ClaimsPrincipal claims)
            => new()
            {
                UserId = claims.GetId(),
                DueDate = input.DueDate.DateToTimestamp(),
                Note = input.Note,
                Title = input.Title ?? "",
            };

        public static UpdateInfoRequest ToRequest(this UpdateTaskInfoInput input, Guid id, ClaimsPrincipal claims)
            => new()
            {
                Id = id.ToString(),
                UserId = claims.GetId(),
                Note = input.Note,
                Title = input.Title ?? "",
            };

        public static ChangeDueDateRequest ToRequest(this ChangeTaskDueDateInput input, Guid id, ClaimsPrincipal claims)
            => new()
            {
                Id = id.ToString(),
                UserId = claims.GetId(),
                DueDate = input.DueDate.DateToTimestamp(),
            };
    }
}
