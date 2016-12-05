using System;
using MongoDB.Bson.Serialization.Attributes;

namespace SignalRChat.Classes
{
	public class MessageEntity
	{
		public string Name { get; set; }
		public string Dialog { get; set; }
		public string Message { get; set; }

		public MessageType Type { get; set; }
		public DateTimeOffset DateTime { get; set; }
	}

	public enum MessageType
	{
		Ordinal = 0,
		HighPriority
	}
}