using Grpc.Core;
using MatBlazor;
using Todo.WebApp.Extensions;
using Todo.WebApp.Models;
using Todo.WebApp.TodoProto.Channel;

namespace Todo.WebApp.Services
{
    public class ToastNotificationService : INotificationService
    {
        private readonly TasksChannel.TasksChannelClient _client;
        private readonly IMatToaster _toaster;
        private readonly string _connectionId;
        private readonly Dictionary<string, MatToast> _inProgressToasts;

        public ToastNotificationService(TasksChannel.TasksChannelClient client, IMatToaster toaster)
        {
            _client = client;
            _toaster = toaster;
            _connectionId = Guid.NewGuid().ToString();
            _inProgressToasts = new Dictionary<string, MatToast>();
            Console.WriteLine("ToastNotificationService initialized.");
        }

        public bool IsStarted { get; private set; }

        public async void Start()
        {
            if (IsStarted) return;
            IsStarted = true;
            Console.WriteLine("ToastNotificationService Start execution.");
            try
            {
                using var call = _client.Notifications(new SubscribeRequest()
                {
                    ConnectionId = _connectionId,
                });

                await foreach (var output in call.ResponseStream.ReadAllAsync())
                {
                    if (_inProgressToasts.TryGetValue(output.TodoTask.Id, out var toast))
                    {
                        if (toast != null)
                            _toaster.Remove(toast);

                        _inProgressToasts.Remove(output.TodoTask.Id);
                    }

                    FireEvent(output);
                }
            }
            catch (RpcException e)
            {
                IsStarted = false;
                Console.WriteLine(e);
                await Task.Delay(TimeSpan.FromSeconds(10));
                Start();
            }
            finally
            {
                IsStarted = false;
            }
        }

        private void FireEvent(NotificationOutput output)
        {
            _toaster.Add(
                message: output.Message,
                type: output.Status == NotificationStatus.Succeed
                    ? MatToastType.Success
                    : MatToastType.Warning,
                title: "Done!",
                icon: "thumb_up"
            );

            var model = output.TodoTask.ToModel();

            if (output.Type == ActionType.Create) TaskCreated(this, model);
            if (output.Type == ActionType.Update) TaskUpdated(this, model);
            if (output.Type == ActionType.Delete) TaskDeleted(this, model);
        }

        public void AddToast(string? id, string? type)
        {
            ArgumentNullException.ThrowIfNull(id);
            ArgumentNullException.ThrowIfNull(type);

            var toast = _toaster.Add(
                message: $"{type} in-progress.",
                type: MatToastType.Primary,
                title: "Ongoing...",
                icon: "hourglass_empty",
                configure: options =>
                {
                    options.ShowProgressBar = true;
                    options.VisibleStateDuration = 30000;
                }
            );

            _inProgressToasts.TryAdd(id, toast);
        }

        public void AddErrorToast(ProblemDetails problemDetails)
        {
            var toast = _toaster.Add(
                message: problemDetails.Detail ?? problemDetails.Title,
                type: MatToastType.Danger,
                title: "Error",
                icon: "hourglass_empty",
                configure: options =>
                {
                    options.ShowProgressBar = true;
                    options.VisibleStateDuration = 3000;
                }
            );
        }

        public event EventHandler<TodoTaskOutput> TaskCreated = delegate { };
        public event EventHandler<TodoTaskOutput> TaskUpdated = delegate { };
        public event EventHandler<TodoTaskOutput> TaskDeleted = delegate { };
    }
}
