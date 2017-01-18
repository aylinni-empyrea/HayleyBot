using System;
using System.Collections.Generic;
using SlackAPI.WebSocketMessages;

namespace HayleyBot
{
	public class Command
	{
		/// <summary>
		///   The command's delegeate which the command event will trigger.
		/// </summary>
		private readonly CommandModule.CommandDelegate CommandDelegate;


		public Command(string permission, CommandModule.CommandDelegate commandDelegate, params string[] aliases)
		{
			if (commandDelegate == null)
				throw new ArgumentNullException(nameof(commandDelegate));
			if (aliases == null || aliases.Length < 1)
				throw new ArgumentException("names");

			CommandDelegate = commandDelegate;
			HelpText = "No help available.";
			HelpDesc = null;
			Aliases = new List<string>(aliases);
			Permission = permission;
		}

		/// <summary>
		///   Name of the command.
		/// </summary>
		public string Name => Aliases[0];

		/// <summary>
		///   Aliases of the command.
		/// </summary>
		public List<string> Aliases { get; protected set; }

		/// <summary>
		///   Short information of the command.
		/// </summary>
		public string HelpText { get; set; }

		/// <summary>
		///   The syntax of the command.
		/// </summary>
		public string SyntaxFormat { get; set; }

		/// <summary>
		///   Detailed description of the command.
		/// </summary>
		public string[] HelpDesc { get; set; }

		/// <summary>
		///   The required permission to execute the command.
		/// </summary>
		public string Permission { get; protected set; }

		/// <summary>
		///   The permission to bypass channel command restriction.
		/// </summary>
		public string ChannelBypassPermission { get; set; }

		/// <summary>
		///   The list of channels where command cannot be executed.
		/// </summary>
		public List<string> RestrictedChannels { get; set; } = new List<string>();

		public bool Execute(string commandName, List<string> parameters, User user, NewMessage messageData)
		{
			if (!user.CanExecuteCommand(commandName))
				return false;

			try
			{
				Bot.Log.Info($"{user.SlackUser.name} has executed: {commandName} {string.Join(" ", parameters)}");
				CommandDelegate(new CommandArgs(commandName, parameters, user, messageData));
			}
			catch (Exception ex)
			{
				Bot.Log.Error($"Unexpected error while executing command \"{commandName}\" by \"{user.SlackUser.name}\"!");
				Bot.Log.Error(ex.ToString());
				return false;
			}
			return true;
		}

		public bool HasAlias(string name)
		{
			return Aliases.Contains(name);
		}

		public bool CanExecuteInChannel(string channel)
		{
			return !RestrictedChannels.Contains(channel);
		}
	}
}