using System;
using Microsoft.Framework.OptionsModel;
using MongoDB.Driver;
using SignalRChat.Classes;

namespace SignalRChat.Domain
{
	public class Repository : IRepository
	{
		private const string Messages = "messages";
		private readonly IMongoDatabase _database;

		public Repository(IOptions<StorageSettings> storageSettings)
		{
			var client = new MongoClient(storageSettings.Options.MongoConnection);
			_database = client.GetDatabase(storageSettings.Options.Database);
		}

		public void StoreMessage(string name, string dialog, string message)
		{
#if !DEBUG
			_database.GetCollection<MessageEntity>(Messages).InsertOne(new MessageEntity
			{
				Name = name,
				Message = message,
				Dialog = dialog,
				DateTime = DateTimeOffset.UtcNow
			});
#endif
		}

		public void StoreHpMessage(string name, string dialog, string message)
		{
#if !DEBUG
			_database.GetCollection<MessageEntity>(Messages).InsertOne(new MessageEntity
			{
				Name = name,
				Message = message,
				Dialog = dialog,
				Type = MessageType.HighPriority,
				DateTime = DateTimeOffset.UtcNow
			});
#endif
		}
	}
}