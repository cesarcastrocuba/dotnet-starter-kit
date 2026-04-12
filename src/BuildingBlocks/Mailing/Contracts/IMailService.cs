using FSH.Framework.Mailing.Messages;

namespace FSH.Framework.Mailing.Contracts;
public interface IMailService
{
    Task SendAsync(MailRequest request, CancellationToken ct);
}