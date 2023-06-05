using System.Net;
using System.Net.Mail;

namespace SmtpLibrary
{
	public class SmtpClientFactory
	{
		private readonly string _hostOrIp;
		private readonly int _port;
		private readonly bool _useSSL;
		private readonly ICredentialsByHost _credentials;

		public SmtpClientFactory(string hostOrIp, int port, bool useSSL, string username, string password)
		{			
			_hostOrIp = hostOrIp;
			_port = port;
			_useSSL = useSSL;
			_credentials = new NetworkCredential(username, password);
		}

		public SmtpClient CreateSmtpClient()
		{		
			return new SmtpClient() { Host = _hostOrIp, Port = _port, EnableSsl = _useSSL, Credentials = _credentials };
		}
	}
}
