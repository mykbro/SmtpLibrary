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
			//## config data
			int numSenders = 3;		//the number of concurrent requests we want to simulate
			
			string myEmailAddress = "mymail@mydomain.com";
			string password = "1234";
			string host = "smtp.blablabla.com";
			string recipientEmailAddress = "anothermail@anotherdomain.com";
			int poolStartingSize = 0;
            int poolMaxSize = 3;

            string subject = "Prova prova";
            string body = "<html><body><h1>AIUTOOOO</h1></body></html>";
			
			//

            SmtpClientFactory fact = new SmtpClientFactory(host, 587, true, myEmailAddress, password);
			SmtpClientPoolSender smtpPool = new SmtpClientPoolSender(fact, poolStartingSize, poolMaxSize);
			
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