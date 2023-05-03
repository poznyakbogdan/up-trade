using API.Models;
using FluentValidation;

namespace API.Validation;

public class PostBalancesValidator : AbstractValidator<PostBalances>
{
    public PostBalancesValidator()
    {
        RuleFor(x => x)
            .Must(x => x.Count == x.Distinct().Count())
            .WithMessage("Duplications are not allowed");
    }
}