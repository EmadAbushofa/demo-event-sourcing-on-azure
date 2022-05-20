using Microsoft.Extensions.DependencyInjection;
using Moq;
using Todo.Command.Abstraction;
using Todo.Command.Infrastructure.Query;
using Todo.Command.Test.FakeServices;

namespace Todo.Command.Test.Helpers
{
    public static class ServiceCollectionExtensions
    {
        public static void ReplaceWithInMemoryEventStore(this IServiceCollection services)
        {
            var eventStore = services.Single(s => s.ServiceType == typeof(IEventStore));
            services.Remove(eventStore);

            services.AddSingleton<IEventStore, InMemoryEventStore>();
        }

        public static void DisableQueryDuplicateDetection(this IServiceCollection services)
        {
            var mock = new Mock<ITodoQuery>();

            mock.Setup(s => s.SimilarTitleExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            services.AddTransient(s => mock.Object);
        }

        public static void MockDuplicateTitleDetection(this IServiceCollection services, string duplicateTitle)
        {
            var mock = new Mock<ITodoQuery>();

            mock.Setup(s => s.SimilarTitleExistsAsync(It.Is<string>(title => title == duplicateTitle)))
                .ReturnsAsync(true);

            mock.Setup(s => s.SimilarTitleExistsAsync(It.Is<string>(title => title != duplicateTitle)))
                .ReturnsAsync(false);

            services.AddTransient(s => mock.Object);
        }
    }
}