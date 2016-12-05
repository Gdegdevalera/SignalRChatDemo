using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using SignalRChat.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRChat.Hubs
{
    [HubName("ChatHub")]
    public class ChatHub : Hub
    {
        private IRepository _repository;

        /// <summary>
        /// Example dependency-injected hub
        /// </summary>
        /// <param name="repository"></param>
        public ChatHub(IRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Maintain a list of connected users
        /// </summary>
        public static Dictionary<string, string> Users = new Dictionary<string, string>(); // ConnectionId => User
		public static Dictionary<string, string> Connections = new Dictionary<string, string>(); // User => ConnectionId

		public void Send(string name, string dialog, string message)
		{
			_repository.StoreMessage(name, dialog, message);
			Send(name, dialog, message, (x, p1, p2, p3) => x.addNewMessageToPage(p1, p2, p3));
		}

	    public void Sendhp(string name, string dialog, string message)
		{
			_repository.StoreHpMessage(name, dialog, message);
			Send(name, dialog, message, (x, p1, p2, p3) => x.addNewHpMessageToPage(p1, p2, p3));
		}

		public void SendImg(string name, string dialog, string imgName)
		{
			Send(name, dialog, imgName, (x, p1, p2, p3) => x.addNewImgMessageToPage(p1, p2, p3));
		}

		public void SendAudio(string name, string dialog, string audioName)
		{
			Send(name, dialog, audioName, (x, p1, p2, p3) => x.addNewAudioMessageToPage(p1, p2, p3));
		}

		public void SendVideo(string name, string dialog, string videoName)
		{
			Send(name, dialog, videoName, (x, p1, p2, p3) => x.addNewVideoMessageToPage(p1, p2, p3));
		}

		/// <summary>
		/// Maintain a count of connected users
		/// </summary>
		/// <param name="count"></param>
		public void Count(int count)
        {
            Clients.All.updateUsersOnlineCount(count);
            GetUsersOnline();
        }

        /// <summary>
        /// Subscribe an active online user by adding to list
        /// </summary>
        /// <param name="displayName"></param>
        public void Subscribe(string displayName)
        {
            Users.Add(Context.ConnectionId, displayName);
			Connections.Add(displayName, Context.ConnectionId);
            Count(Users.Count);
        }

        /// <summary>
        /// Retrive a list of online users
        /// </summary>
        /// <returns></returns>
        public List<string> GetUsersOnline()
        {
            var onlineUsers = Users.Values.ToList();
            Clients.All.getOnlineUsers(onlineUsers);
            return onlineUsers;
        }

        public override Task OnConnected()
        {
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
	        if (Users.Keys.Contains(this.Context.ConnectionId))
	        {
		        var user = Users[this.Context.ConnectionId];
				Users.Remove(this.Context.ConnectionId);
		        Connections.Remove(user);
	        }

            // Send the current count of users
            Count(Users.Count);

            return base.OnDisconnected(false);
        }

        public override Task OnReconnected()
        {
            GetUsersOnline();

            return base.OnReconnected();
		}

		private void Send(string name, string dialog, string message, Action<dynamic, object, object, object> sendAction)
		{
			if (dialog == null)
			{
				sendAction(Clients.All, name, null, message);
			}
			else
			{
				string dialogId;
				if (Connections.TryGetValue(dialog, out dialogId))
				{
					sendAction(Clients.Client(dialogId), name, name, message);
					sendAction(Clients.Client(Context.ConnectionId), name, dialog, message);
				}
			}
		}
	}
}
