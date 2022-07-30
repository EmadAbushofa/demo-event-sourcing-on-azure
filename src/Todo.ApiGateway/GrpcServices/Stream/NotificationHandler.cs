using Todo.ApiGateway.Resources;
using Todo.ApiGateway.TodoProto.Channel;
using Todo.ApiGateway.TodoProto.Query;

namespace Todo.ApiGateway.GrpcServices.Stream
{
    public class NotificationHandler
    {
        public NotificationOutput? Handle(NotificationResponse response)
        {
            var (message, status, actionType) = response.Type switch
            {
                nameof(Messages.TaskCreated) => HandleCreated(response),
                nameof(Messages.TaskInfoUpdated) => HandleInfoUpdated(response),
                nameof(Messages.TaskDueDateChanged) => HandleDueDateChanged(response),
                nameof(Messages.TaskCompleted) => HandleCompleted(),
                nameof(Messages.TaskUncompleted) => HandleUncompleted(),
                nameof(Messages.TaskDeleted) => HandleDeleted(),
                _ => (string.Empty, (NotificationStatus?)null, (ActionType?)null),
            };

            if (status == null || actionType == null) return null;

            return new NotificationOutput()
            {
                TodoTask = new TodoProto.Channel.TaskOutput()
                {
                    Id = response.Task.Id,
                    DueDate = response.Task.DueDate,
                    DuplicateTitle = response.Task.DuplicateTitle,
                    IsCompleted = response.Task.IsCompleted,
                    Note = response.Task.Note,
                    Title = response.Task.Title,
                },
                Type = actionType.Value,
                Message = message,
                Status = status.Value,
            };
        }

        private static (string, NotificationStatus, ActionType) HandleCreated(NotificationResponse response) =>
            response.Task.DuplicateTitle
                ? (Messages.TaskCreatedWithDuplicateTitle, NotificationStatus.Warning, ActionType.Create)
                : (Messages.TaskCreated, NotificationStatus.Succeed, ActionType.Create);

        private static (string, NotificationStatus, ActionType) HandleInfoUpdated(NotificationResponse response) =>
            response.Task.DuplicateTitle
                ? (Messages.TaskInfoUpdatedWithDuplicateTitle, NotificationStatus.Warning, ActionType.Update)
                : (Messages.TaskInfoUpdated, NotificationStatus.Succeed, ActionType.Update);

        private static (string, NotificationStatus, ActionType) HandleDueDateChanged(NotificationResponse response) =>
            (string.Format(Messages.TaskDueDateChanged, response.Task.DueDate.ToDateTime().Date), NotificationStatus.Succeed, ActionType.Update);

        private static (string, NotificationStatus, ActionType) HandleCompleted() =>
            (Messages.TaskCompleted, NotificationStatus.Succeed, ActionType.Update);

        private static (string, NotificationStatus, ActionType) HandleUncompleted() =>
            (Messages.TaskUncompleted, NotificationStatus.Succeed, ActionType.Update);

        private static (string, NotificationStatus, ActionType) HandleDeleted() =>
            (Messages.TaskDeleted, NotificationStatus.Succeed, ActionType.Delete);
    }
}
