﻿using Todo.Command.Events;
using Todo.Command.Events.DataTypes;
using Todo.Command.Features.Create;

namespace Todo.Command.Extensions
{
    public static class EventsExtensions
    {
        public static TaskCreatedEvent ToEvent(this CreateTaskCommand command)
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
    }
}