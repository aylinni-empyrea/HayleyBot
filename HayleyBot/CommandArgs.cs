using System;
using System.Collections.Generic;
using SlackAPI.WebSocketMessages;

namespace HayleyBot
{
	public class CommandArgs : EventArgs
	{
		public CommandArgs(string commandName, List<string> parameters, User executingUser, NewMessage messageData)
		{
			CommandName = commandName;
			Parameters = parameters;
			ExecutingUser = executingUser;
			MessageData = messageData;
		}

		public string CommandName { get; private set; }
		public List<string> Parameters { get; set; }
		public User ExecutingUser { get; set; }
		public NewMessage MessageData { get; private set; }
	}
}