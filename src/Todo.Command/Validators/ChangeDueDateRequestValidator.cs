using FluentValidation;
using Todo.Command.TodoProto;

namespace Todo.Command.Validators
{
    public class ChangeDueDateRequestValidator : AbstractValidator<ChangeDueDateRequest>
    {
        public ChangeDueDateRequestValidator()
        {
            RuleFor(c => c.Id)
                .NotEmpty()
                .Must(id => Guid.TryParse(id, out _));

            RuleFor(c => c.UserId)
                .NotEmpty();

            RuleFor(c => c.DueDate)
                .Must(dueDate => dueDate.ToDateTime().Year < 2200)
                .Must(dueDate => dueDate.ToDateTime().Year > 1800)
                .WithMessage("Due Date should be within a valid range.");
        }
    }
}
