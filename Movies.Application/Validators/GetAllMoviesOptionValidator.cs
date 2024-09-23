using FluentValidation;
using Movies.Application.Models;

namespace Movies.Application.Validators;

public class GetAllMoviesOptionValidator : AbstractValidator<GetAllMoviesOptions>
{
    public GetAllMoviesOptionValidator()
    {
        RuleFor(x => x.YearOfRelease)
            .LessThanOrEqualTo(DateTime.UtcNow.Year);
    }
}