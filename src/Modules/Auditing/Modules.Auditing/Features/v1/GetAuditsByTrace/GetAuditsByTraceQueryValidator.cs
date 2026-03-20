using FluentValidation;
using FSH.Modules.Auditing.Contracts.v1.GetAuditsByTrace;

namespace FSH.Modules.Auditing.Features.v1.GetAuditsByTrace;

public sealed class GetAuditsByTraceQueryValidator : AbstractValidator<GetAuditsByTraceQuery>
{
    public GetAuditsByTraceQueryValidator()
    {
        RuleFor(q => q.TraceId)
            .NotEmpty();

        RuleFor(q => q)
            .Must(q => !q.FromOnUtc.HasValue || !q.ToOnUtc.HasValue || q.FromOnUtc <= q.ToOnUtc)
            .WithMessage("FromOnUtc must be less than or equal to ToOnUtc.");
    }
}

