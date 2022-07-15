using MediatR;
using Todo.Query.Abstractions;

namespace Todo.Query.QueryHandlers.Filter
{
    public class FilterQueryHandler : IRequestHandler<FilterQuery, FilterResult>
    {
        private readonly IUnitOfWork _unitOfWork;

        public FilterQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<FilterResult> Handle(FilterQuery request, CancellationToken cancellationToken)
        {
            return _unitOfWork.Tasks.FilterAsync(request, cancellationToken);
        }
    }
}
