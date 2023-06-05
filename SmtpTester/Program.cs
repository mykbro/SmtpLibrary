using System.Net.Mail;
using System.Net;
using SmtpLibrary;
using System.Security.Cryptography;

namespace SmtpTester
{
	public static class Program
	{
		static void Main(string[] args)
		{
			int numSenders = 3;
			
			string myEmailAddress = "";
			string password = "";
			string host = "";
			string recipientEmailAddress = "";
			
			SmtpClientFactory fact = new SmtpClientFactory(host, 587, true, myEmailAddress, password!);
			SmtpClientPoolSender smtpPool = new SmtpClientPoolSender(fact, 0, 3);

			string subject = "Prova prova";
			string body = "<html><body><h1>AIUTOOOO</h1></body></html>";
			MailMsgFactory msgFactory = new MailMsgFactory(new MailAddress(myEmailAddress), subject, body, true);

			Task[] tasks = new Task[numSenders];
			MailAddress mailAddress = new MailAddress(recipientEmailAddress);
			for (int i = 0; i < numSenders; i++)
			{								
				Task t = Task.Run(() => StartSendingMailAsync(mailAddress, smtpPool, msgFactory));
				tasks[i] = t;				
			}
			
			Task.WaitAll(tasks);
		}

		static async Task StartSendingMailAsync(MailAddress sender, SmtpClientPoolSender smtpPool, MailMsgFactory msgFactory)
		{
			while (true)
			{
				await Task.Delay(RandomNumberGenerator.GetInt32(10000));
				await smtpPool.SendMailAsync(msgFactory.CreateMessage(sender));
			}
		}



	}
}