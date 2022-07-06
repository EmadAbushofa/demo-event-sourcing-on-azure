using FluentValidation;
using Todo.Command.Server.TodoProto;

namespace Todo.Command.Validators
{
    public class UpdateInfoRequestValidator : AbstractValidator<UpdateInfoRequest>
    {
        public UpdateInfoRequestValidator()
        {
            RuleFor(c => c.Id)
                .NotEmpty()
                .Must(id => Guid.TryParse(id, out _));

            RuleFor(c => c.UserId)
                .NotEmpty();

            RuleFor(c => c.Title)
                .NotEmpty();

            RuleFor(c => c.Note)
                .MaximumLength(1000);
        }
    }
}
