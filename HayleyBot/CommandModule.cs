using System.Collections.Generic;
using System.Linq;
using SlackAPI.WebSocketMessages;

namespace HayleyBot
{
	public class CommandModule
	{
		public delegate void CommandDelegate(CommandArgs args);

		public static List<Command> Commands { get; set; } = new List<Command>();

		public static void ProcessCommand(User user, string[] parameters, NewMessage messageData)
		{
			string commandName = parameters[0];
			parameters = parameters.Skip(1).ToArray();

			var commands = Commands.FindAll(c => c.HasAlias(commandName));

			if (commands.Count == 0)
			{
				Bot.Client.SendMessage(null, messageData.channel,
					"Invalid command entered! Type /help for a list of available commands!");
				return;
			}
			foreach (var command in commands)
				if (!user.CanExecuteCommand(command))
				{
					Bot.Client.SendMessage(null, messageData.channel, "You have no permission to execute this command!");
					return;
				}
				else if (!command.CanExecuteInChannel(messageData.channel) && !user.HasPermission(command.ChannelBypassPermission))
				{
					Bot.Client.SendMessage(null, messageData.channel,
						"You have no permission to execute this command in this channel!");
					return;
				}
				else
				{
					command.Execute(commandName, parameters.ToList(), user, messageData);
					return;
				}
			return;
		}
	}
}