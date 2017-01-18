using System;
using System.Collections.Generic;
using System.Linq;

namespace HayleyBot
{
	public class User
	{
		public static User Self => new User(Bot.Client.MySelf.id);

		public bool IsSelf => ID == Self.ID;

		public User(string id)
		{
			ID = id;
		}

		public SlackAPI.User SlackUser => Bot.Client.Users.Find(x => x.id == ID);

		public string ID { get; }

		public List<string> Permissions { get; set; } = Commands.DefaultPermissions;

		public bool IsAdmin { get; set; }

		public bool CanExecuteCommand(Command command)
			=> !string.IsNullOrEmpty(command.Permission) && HasPermission(command.Permission);

		/// <summary>
		///   Determines if the player can execute a command, using a command name in string.
		/// </summary>
		/// <param name="commandName">The command's name we want to check.</param>
		/// <returns>Returns true/false, or throws exception if command is not found.</returns>
		public bool CanExecuteCommand(string commandName)
		{
			var command = CommandModule.Commands.Where(c => c.Aliases.Contains(commandName)).Select(c => c).FirstOrDefault();
			return command != null && CanExecuteCommand(command);
		}

		public bool HasPermission(string permission) => Permissions.Contains(permission);

	}
}