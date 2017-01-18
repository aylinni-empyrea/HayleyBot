using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;
using SlackAPI;
using SlackAPI.WebSocketMessages;

namespace HayleyBot
{
	public class Bot
	{
		/// <summary>
		///   The name of the Bot.
		/// </summary>
		public static string BotName = "HayleyBot";

		/// <summary>
		///   Log4Net logger.
		/// </summary>
		public static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		///   The Bot access token. Used for bot actions.
		/// </summary>
		public static string BotAccessToken;

		/// <summary>
		///   User access token. Used for deleting messages.
		/// </summary>
		public static string AccessToken;

		/// <summary>
		///   The client connection between the Bot and Slack server.
		/// </summary>
		public static SlackSocketClient Client { get; set; }

		/// <summary>
		///   List of slack members.
		///   Key: Slack user ID.
		///   Value: User object.
		/// </summary>
		public static Dictionary<string, User> Users { get; internal set; } = new Dictionary<string, User>();

		/// <summary>
		///   A list of initialized commands.
		/// </summary>
		/// <summary>
		///   Initiates a connection between the bot and Slack server.
		/// </summary>
		public static void Connect()
		{
			Client = new SlackSocketClient(BotAccessToken);
			Client.OnMessageReceived += OnMessageReceived;
			Client.Connect(ClientConnected);
		}

		#region Events

		public static event Action<LoginResponse> ClientConnected;

		/// <summary>
		///   Gets invoked if Bot is in the channel where text is sent, or if Bot receives a PM.
		/// </summary>
		private static void OnMessageReceived(NewMessage message)
		{
			var commandParams = message.text.Split(' ').ToArray();
			if (string.Equals(commandParams[0], BotName, StringComparison.CurrentCultureIgnoreCase))
				if (commandParams.Length > 1)
					CommandModule.ProcessCommand(Users.ContainsKey(message.user) ? Users[message.user] : new User(message.user),
						commandParams.Skip(1).ToArray(), message);

			Console.WriteLine(message.text);
		}

		#endregion

		public static void Initialize()
		{
			Commands.InitializeCommands();
		}

		public static void Run()
		{
			Initialize();
			while (true)
			{
				string input = Console.ReadLine();
				if (input != null)
				{
					var args = input.Split(' ');
					string command = args[0];
					args = args.Skip(1).ToArray();

					switch (command)
					{
						case "test":
						{
							Client.PostMessage(null, "#general", "Hy I'm 7 gimme candy", "HayleyBot");
							break;
						}
						case "connect":
						{
							Connect();
							Console.WriteLine("Attempted to reconnect? idk");
							break;
						}
					}
				}
			}
		}
	}
}