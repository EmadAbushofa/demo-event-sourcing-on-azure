using Todo.WebApp.Models;
using Todo.WebApp.TodoProto.Channel;

namespace Todo.WebApp.Extensions
{
    public static class ProtoExtensions
    {
        public static TodoTaskOutput ToModel(this TaskOutput output) =>
            new()
            {
                Id = output.Id,
                DueDate = output.DueDate.ToDateTime(),
                DuplicateTitle = output.DuplicateTitle,
                IsCompleted = output.IsCompleted,
                Note = output.Note,
                Title = output.Title,
            };
    }
}
