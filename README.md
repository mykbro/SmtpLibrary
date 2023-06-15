# SmtpLibrary

## Disclaimer
This codebase is made for self-teaching and educational purposes only.
Many features like input validation, object disposed checks, some exception handling, etc... are mostly missing.
As such this codebase cannot be considered production ready.

## What's this ?
This library provides a thread-safe pooled Smtp client service leveraging the SmtpClient class implemented in System.Net.Mail.
A testing app is also included.

## How does it work ?
The SmtpClientPoolSender uses a SmtpClientFactory to create its clients.  
Based on the number of requests at a certain time, the actual pool size and the configured maximum pool size, the SmtpClientPoolSender will decide either to
reuse an existing SmtpClient, create a new one, dispose the one just used or recycle it.

For more details please check the SmtpClientPoolSender source code.


## How should I use this ?
For .Net Core Web apps SmtpClientPoolSender can be registered as a singleton for the Dependency Injection framework, using:

	int startingSize = ... ;
	int maxSize = ... ;
	var mySmtpClientFactory = new SmtpClientFactory(...);
	var myInstance = new SmtpClientPoolSender(mySmtpClientFactory, startingSize, maxSize);

	builder.Services.AddSingleton<IMailSender>(myInstance);

To test the library using the provided driver app please configure the following section in Program.Main:

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
		...
	}

