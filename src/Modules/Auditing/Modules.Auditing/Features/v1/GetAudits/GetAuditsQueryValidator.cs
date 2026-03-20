using FluentValidation;
using FSH.Framework.Web.Validation;
using FSH.Modules.Auditing.Contracts.v1.GetAudits;

namespace FSH.Modules.Auditing.Features.v1.GetAudits;

public sealed class GetAuditsQueryValidator : AbstractValidator<GetAuditsQuery>
{
    public GetAuditsQueryValidator()
    {
        Include(new PagedQueryValidator<GetAuditsQuery>());

        RuleFor(q => q)
            .Must(q => !q.FromOnUtc.HasValue || !q.ToOnUtc.HasValue || q.FromOnUtc <= q.ToOnUtc)
            .WithMessage("FromOnUtc must be less than or equal to ToOnUtc.");
    }
}

