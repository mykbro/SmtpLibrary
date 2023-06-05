using System.Net.Mail;

namespace SmtpLibrary
{
	public interface IMailSender
	{
		public Task SendMailAsync(MailMessage msg);
	}		
}
