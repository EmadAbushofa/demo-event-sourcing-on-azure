using FluentValidation;
using Todo.WebApp.ViewModels;

namespace Todo.WebApp.Validators
{
    public class CreateViewModelValidator : AbstractValidator<CreateViewModel>
    {
        public CreateViewModelValidator()
        {
            RuleFor(c => c.Title)
                .Must((model, _) => !model.DuplicateTitle)
                .WithMessage("There is another opened task with the same title!");

            RuleFor(c => c.Title)
                .NotEmpty();

            RuleFor(c => c.DueDate)
                .NotNull()
                .Must(dueDate => dueDate.GetValueOrDefault().Year < 2200)
                .Must(dueDate => dueDate.GetValueOrDefault().Year > 1800)
                .WithMessage("Due Date should be within a valid range.");

            RuleFor(c => c.Note)
                .MaximumLength(1000);
        }
    }
}
