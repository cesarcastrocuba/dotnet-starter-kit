using FluentValidation;
using FSH.Modules.Auditing.Contracts.v1.GetAuditSummary;

namespace FSH.Modules.Auditing.Features.v1.GetAuditSummary;

public sealed class GetAuditSummaryQueryValidator : AbstractValidator<GetAuditSummaryQuery>
{
    public GetAuditSummaryQueryValidator()
    {
        RuleFor(q => q)
            .Must(q => !q.FromOnUtc.HasValue || !q.ToOnUtc.HasValue || q.FromOnUtc <= q.ToOnUtc)
            .WithMessage("FromOnUtc must be less than or equal to ToOnUtc.");
    }
}

