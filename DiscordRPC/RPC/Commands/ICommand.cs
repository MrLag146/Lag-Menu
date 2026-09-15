using Lagmenu.DiscordRPC.RPC.Payload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lagmenu.DiscordRPC.RPC.Commands
{
    internal interface ICommand
	{
		IPayload PreparePayload(long nonce);
	}
}
