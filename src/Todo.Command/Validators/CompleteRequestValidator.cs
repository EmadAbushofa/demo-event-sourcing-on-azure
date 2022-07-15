using FluentValidation;
using Todo.Command.TodoProto;

namespace Todo.Command.Validators
{
    public class CompleteRequestValidator : AbstractValidator<CompleteRequest>
    {
        public CompleteRequestValidator()
        {
            RuleFor(c => c.Id)
                .NotEmpty()
                .Must(id => Guid.TryParse(id, out _));

            RuleFor(c => c.UserId)
                .NotEmpty();
        }
    }
}
