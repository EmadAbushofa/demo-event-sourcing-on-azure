﻿using Todo.Command.CommandHandlers.ChangeDueDate;
using Todo.Command.CommandHandlers.Complete;
using Todo.Command.CommandHandlers.Create;
using Todo.Command.CommandHandlers.Delete;
using Todo.Command.CommandHandlers.Uncomplete;
using Todo.Command.CommandHandlers.UpdateInfo;
using Todo.Command.Events;
using Todo.Command.Events.DataTypes;

namespace Todo.Command.Extensions
{
    public static class EventsExtensions
    {
        public static TaskCreated ToEvent(this CreateTaskCommand command)
            => new(
                aggregateId: Guid.NewGuid(),
                sequence: 1,
                userId: command.UserId,
                data: new TaskCreatedData(
                    Title: command.Title,
                    DueDate: command.DueDate,
                    Note: command.Note
                )
            );

        public static TaskInfoUpdated ToEvent(this UpdateTaskInfoCommand command, int sequence)
            => new(
                aggregateId: command.Id,
                sequence: sequence,
                userId: command.UserId,
                data: new TaskInfoUpdatedData(
                    Title: command.Title,
                    Note: command.Note
                )
            );

        public static TaskDueDateChanged ToEvent(this ChangeDueDateCommand command, int sequence)
            => new(
                aggregateId: command.Id,
                sequence: sequence,
                userId: command.UserId,
                data: new TaskDueDateChangedData(
                    DueDate: command.DueDate
                )
            );

        public static TaskCompleted ToEvent(this CompleteCommand command, int sequence)
            => new(
                aggregateId: command.Id,
                sequence: sequence,
                userId: command.UserId,
                data: new object()
            );

        public static TaskUncompleted ToEvent(this UncompleteCommand command, int sequence)
            => new(
                aggregateId: command.Id,
                sequence: sequence,
                userId: command.UserId,
                data: new object()
            );

        public static TaskDeleted ToEvent(this DeleteCommand command, int sequence)
            => new(
                aggregateId: command.Id,
                sequence: sequence,
                userId: command.UserId,
                data: new object()
            );
    }
}
