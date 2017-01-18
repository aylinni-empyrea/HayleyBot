using System;
using System.Collections.Generic;
using System.Text;

namespace HayleyBot
{
	public static class Commands
	{
		public static readonly List<string> DefaultPermissions = new List<string>
		{
			"core.help",
			"core.admin.exit"
		};

		public static void InitializeCommands()
		{
			Action<Command> add = cmd => { CommandModule.Commands.Add(cmd); };

			add(new Command("core.help", Help, "help")
			{
				SyntaxFormat = "help <command>",
				HelpText = "Shows a list of commands. Or detailed information of a command."
			});

			add(new Command("core.admin.exit", Exit, "exit", "shutdown"));
		}

		public static void Help(CommandArgs args)
		{
			var sb = new StringBuilder();
			foreach (var command in CommandModule.Commands)
				sb.Append($"{command.SyntaxFormat} - {command.HelpText}\n");

			if (!args.ExecutingUser.IsSelf)
			{
				Bot.Client.SendMessage(null, args.MessageData.channel, sb.ToString());
			}
			else
			{
				Console.WriteLine(sb.ToString());
			}
		}

		public static void Exit(CommandArgs args)
		{
			Program.ExitSignal = true;
		}
	}
}