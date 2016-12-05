namespace SignalRChat.Domain
{
    public interface IRepository
    {
	    void StoreMessage(string name, string dialog, string message);
	    void StoreHpMessage(string name, string dialog, string message);
    }
}