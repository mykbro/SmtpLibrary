using System.Net.Mail;

namespace SmtpLibrary
{
	public class MailMsgFactory
	{
		private readonly MailAddress _senderAddress;
		private readonly string _subject;
		private readonly string _body;
		private readonly bool _isBodyHtml;

		public MailMsgFactory(MailAddress senderAddress, string subject, string body, bool isBodyHtml)
        {
            _senderAddress = senderAddress;
			_subject = subject;
			_body = body;		
			_isBodyHtml = isBodyHtml;
        }

		public MailMessage CreateMessage(MailAddress recipientAddress)
		{
			return new MailMessage(_senderAddress, recipientAddress) { Subject = _subject, Body = _body, IsBodyHtml = _isBodyHtml };			
		}
		
    }
}
