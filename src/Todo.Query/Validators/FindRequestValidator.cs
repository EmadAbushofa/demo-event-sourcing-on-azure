using FluentValidation;
using Todo.Query.Server.TodoProto;

namespace Todo.Query.Validators
{
    public class FindRequestValidator : AbstractValidator<FindRequest>
    {
        public FindRequestValidator()
        {
            RuleFor(r => r.Id)
                .Must(id => Guid.TryParse(id, out _));
        }
    }
}
