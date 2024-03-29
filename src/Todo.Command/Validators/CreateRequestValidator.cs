﻿using FluentValidation;
using Todo.Command.TodoProto;

namespace Todo.Command.Validators
{
    public class CreateRequestValidator : AbstractValidator<CreateRequest>
    {
        public CreateRequestValidator()
        {
            RuleFor(c => c.UserId)
                .NotEmpty();

            RuleFor(c => c.Title)
                .NotEmpty();

            RuleFor(c => c.DueDate)
                .Must(dueDate => dueDate.ToDateTime().Year < 2200)
                .Must(dueDate => dueDate.ToDateTime().Year > 1800)
                .WithMessage("Due Date should be within a valid range.");

            RuleFor(c => c.Note)
                .MaximumLength(1000);
        }
    }
}
