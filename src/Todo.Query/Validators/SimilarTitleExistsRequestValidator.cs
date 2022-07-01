using FluentValidation;
using Todo.Query.Server.TodoProto;

namespace Todo.Query.Validators
{
    public class SimilarTitleExistsRequestValidator : AbstractValidator<SimilarTitleExistsRequest>
    {
        public SimilarTitleExistsRequestValidator()
        {
            RuleFor(r => r.UserId)
                .NotEmpty();

            RuleFor(r => r.Title)
                .NotEmpty();
        }
    }
}
