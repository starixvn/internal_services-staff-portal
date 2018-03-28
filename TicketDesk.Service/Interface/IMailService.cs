namespace TicketDesk.Service.Interface
{
    using Domain.Model;
    using Fanex.MailDeliver.Model;

    public interface IMailService
    {
        void Send(MailRecord mailRecord);

        MailRecord ComposeMail(Ticket ticket);

        void ComposeAndSendMail(Ticket ticket);

        bool IsEmailListValid(string emails);

        bool IsEmailValid(string email);
    }
}
