using Microsoft.AspNetCore.Components;
using Todo.WebApp.Extensions;
using Todo.WebApp.Models;

namespace Todo.WebApp.ViewModels
{
    public class FilterQueryViewModel
    {
        private readonly NavigationManager? _navigationManager;
        private int _page = 1;
        private int _size = 25;
        private string? _state = CompletionState.PendingTasks;
        private DateTime? _dueDateFrom;
        private DateTime? _dueDateTo;

        public FilterQueryViewModel()
        {

        }

        public FilterQueryViewModel(NavigationManager navigationManager)
        {
            navigationManager.SetFromQueryStrings(this);
            _navigationManager = navigationManager;
        }

        public int Page
        {
            get => _page;
            set => UpdateQueryStringIfChanged(ref _page, value);
        }
        public int Size
        {
            get => _size;
            set => UpdateQueryStringIfChanged(ref _size, value);
        }
        public string? State
        {
            get => _state;
            set => UpdateQueryStringIfChanged(ref _state, value);
        }
        public DateTime? DueDateFrom
        {
            get => _dueDateFrom;
            set => UpdateQueryStringIfChanged(ref _dueDateFrom, value);
        }
        public DateTime? DueDateTo
        {
            get => _dueDateTo;
            set => UpdateQueryStringIfChanged(ref _dueDateTo, value);
        }

        public string GetFilterModelAsQueryString() =>
            new FilterModel()
            {
                Page = Page,
                Size = Size,
                IsCompleted = State switch
                {
                    CompletionState.CompletedTasks => true,
                    CompletionState.PendingTasks => false,
                    CompletionState.AllTasks => null,
                    _ => false,
                },
                DueDateFrom = DueDateFrom?.ToString("yyyy-MM-dd"),
                DueDateTo = DueDateTo?.ToString("yyyy-MM-dd"),
            }.ToQueryString();

        private void UpdateQueryStringIfChanged<T>(ref T? member, T? value)
        {
            if (!Equals(member, value))
            {
                member = value;
                if (_navigationManager != null)
                {
                    var query = this.ToQueryString();
                    _navigationManager?.SetUrlQueryString(query);
                }
            }
        }
    }
}
