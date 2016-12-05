namespace SignalRChat.Domain
{
    public class StorageSettings
    {
		public string Database { get; set; }
		public string MongoConnection { get; set; }
	}

	public class CommonSettings
	{
		public int MaxUploadImageHeight { get; set; }
		public int MaxUploadImageWidth { get; set; }
	}
}
