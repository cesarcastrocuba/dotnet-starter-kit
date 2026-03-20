using FluentValidation;
using FSH.Modules.Auditing.Contracts.v1.GetSecurityAudits;

namespace FSH.Modules.Auditing.Features.v1.GetSecurityAudits;

public sealed class GetSecurityAuditsQueryValidator : AbstractValidator<GetSecurityAuditsQuery>
{
    public GetSecurityAuditsQueryValidator()
    {
        RuleFor(q => q)
            .Must(q => !q.FromOnUtc.HasValue || !q.ToOnUtc.HasValue || q.FromOnUtc <= q.ToOnUtc)
            .WithMessage("FromOnUtc must be less than or equal to ToOnUtc.");
    }
}

