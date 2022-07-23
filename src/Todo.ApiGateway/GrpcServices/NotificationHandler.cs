using Todo.ApiGateway.Resources;
using Todo.ApiGateway.TodoProto.Channel;
using Todo.ApiGateway.TodoProto.Query;

namespace Todo.ApiGateway.GrpcServices
{
    public class NotificationHandler
    {
        public NotificationOutput? Handle(NotificationResponse response)
        {
            var (message, status) = response.Type switch
            {
                nameof(Messages.TaskCreated) => HandleCreated(response),
                nameof(Messages.TaskInfoUpdated) => HandleInfoUpdated(response),
                nameof(Messages.TaskDueDateChanged) => HandleDueDateChanged(response),
                nameof(Messages.TaskCompleted) => HandleCompleted(),
                nameof(Messages.TaskUncompleted) => HandleUncompleted(),
                nameof(Messages.TaskDeleted) => HandleDeleted(),
                _ => (string.Empty, (NotificationStatus?)null),
            };

            if (status == null) return null;

            return new NotificationOutput()
            {
                Message = message,
                Status = status.Value,
                TodoTaskId = response.Task.Id,
            };
        }

        private static (string, NotificationStatus) HandleCreated(NotificationResponse response) =>
            response.Task.DuplicateTitle
                ? (Messages.TaskCreatedWithDuplicateTitle, NotificationStatus.Warning)
                : (Messages.TaskCreated, NotificationStatus.Succeed);

        private static (string, NotificationStatus) HandleInfoUpdated(NotificationResponse response) =>
            response.Task.DuplicateTitle
                ? (Messages.TaskInfoUpdatedWithDuplicateTitle, NotificationStatus.Warning)
                : (Messages.TaskInfoUpdated, NotificationStatus.Succeed);

        private static (string, NotificationStatus) HandleDueDateChanged(NotificationResponse response) =>
            (string.Format(Messages.TaskDueDateChanged, response.Task.DueDate.ToDateTime().Date), NotificationStatus.Succeed);

        private static (string, NotificationStatus) HandleCompleted() =>
            (Messages.TaskCompleted, NotificationStatus.Succeed);

        private static (string, NotificationStatus) HandleUncompleted() =>
            (Messages.TaskUncompleted, NotificationStatus.Succeed);

        private static (string, NotificationStatus) HandleDeleted() =>
            (Messages.TaskDeleted, NotificationStatus.Succeed);
    }
}
