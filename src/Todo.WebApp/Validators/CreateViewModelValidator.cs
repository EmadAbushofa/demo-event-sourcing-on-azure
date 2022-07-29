using FluentValidation;
using Todo.WebApp.ViewModels;

namespace Todo.WebApp.Validators
{
    public class CreateViewModelValidator : AbstractValidator<CreateViewModel>
    {
        public CreateViewModelValidator()
        {
            RuleFor(m => m.Title)
                .NotEmpty();
        }
    }
}
