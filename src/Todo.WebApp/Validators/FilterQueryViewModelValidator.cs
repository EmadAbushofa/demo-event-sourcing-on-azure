using FluentValidation;
using Todo.WebApp.ViewModels;

namespace Todo.WebApp.Validators
{
    public class FilterQueryViewModelValidator : AbstractValidator<FilterQueryViewModel>
    {
        public FilterQueryViewModelValidator()
        {
        }
    }
}
