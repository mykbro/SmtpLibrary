using System.Net;
using System.Net.Mail;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace SmtpLibrary
{
	public class SmtpClientPoolSender : IDisposable, IMailSender
	{		
		private readonly Queue<SmtpClient> _clientQueue;
		private readonly object _globalLock;		
		private readonly SmtpClientFactory _clientFactory;
		private readonly int _maxSize;

		private int _spawnedClients;
		private bool _disposed;
		private bool _canBeDisposed;


		public SmtpClientPoolSender(SmtpClientFactory clientFactory, int startingSize, int maxSize)
		{
			// a startingSize = 0, maxSize = 0 is possible. We just create and dispose a new client on each request.
			
			if (clientFactory is null || startingSize < 0 || maxSize < startingSize)
			{
				throw new ArgumentException();
			}

			_clientQueue = new Queue<SmtpClient>(startingSize);
			_clientFactory = clientFactory;
			_maxSize = maxSize;
			_globalLock = new object();			
			_disposed = false;
			_canBeDisposed = true;

			for (int i = 0; i < startingSize; i++)
			{
				_clientQueue.Enqueue(_clientFactory.CreateSmtpClient());						
			}

			_spawnedClients = startingSize;
		}

		public void Dispose()
		{
			lock (_globalLock)
			{
				if (_disposed)
				{
					throw new ObjectDisposedException(nameof(SmtpClientPoolSender));
				}
				else
				{
					if (!_canBeDisposed)
					{
						throw new InvalidOperationException();
					}
					else
					{
						// we mark the object as already disposed
						_disposed = true;						
					}
				}				
			}
			
			// if we're here we didn't throw so we can dispose without problems
			while(_clientQueue.Count > 0)
			{
				SmtpClient temp = _clientQueue.Dequeue();
				temp.Dispose();			
			}
		}

		public async Task SendMailAsync(MailMessage msg)
		{
			SmtpClient? clientToUse = null;
			bool clientFound = false;
			bool keepClient = false;			
			
			lock(_globalLock)
			{	
				//we check if the pool is disposed. If not we mark the object as not disposable in the meantime
				if (_disposed)
				{
					throw new ObjectDisposedException(nameof(SmtpClientPoolSender));
				}					
				else
				{
					_canBeDisposed = false;

					// we check if there's a client available in the pool
					clientFound = _clientQueue.TryDequeue(out clientToUse);
					// if not we take the chance to increase _spawnedClients without acquiring another lock later
					if (!clientFound)
					{
						_spawnedClients++;
					}
				}
			}

			// if the pool was empty we create a new one 
			if (!clientFound)
			{
				clientToUse = _clientFactory.CreateSmtpClient();			
			}

			
			// we send the msg
			await clientToUse!.SendMailAsync(msg);
			
			/*
			await Task.Delay(1000);
			Console.WriteLine("Sent to " + msg.To[0].Address);
			*/

			// we check if we need to keep the client or we have to dipose it
			lock (_globalLock)
			{				
				if (_spawnedClients <= _maxSize)
				{
					keepClient = true;
					_clientQueue.Enqueue(clientToUse);					
				}
				else
				{
					_spawnedClients--;
				}

				// we mark the pool as Disposable again
				_canBeDisposed = true;
			}

			// we dispose the client if we reached the max size, if we disposed the pool in the meantime clientToUse wasn't in the queue so it must be disposed anyway
			if (!keepClient)
			{
				clientToUse.Dispose();			
			}
		}
	}
}
