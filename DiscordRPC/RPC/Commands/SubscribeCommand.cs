using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lagmenu.DiscordRPC.RPC.Payload;

namespace Lagmenu.DiscordRPC.RPC.Commands
{
	internal class SubscribeCommand : ICommand
	{
		public ServerEvent Event { get; set; }
		public bool IsUnsubscribe { get; set; }
		
		public IPayload PreparePayload(long nonce)
		{
			return new EventPayload(nonce)
			{
				Command = IsUnsubscribe ? Command.Unsubscribe : Command.Subscribe,
				Event = Event
			};
		}
	}
}
