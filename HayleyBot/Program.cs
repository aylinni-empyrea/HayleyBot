using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using log4net.Config;
using Mono.Options;
using SlackAPI.WebSocketMessages;

namespace HayleyBot
{
	internal enum ExitCodes
	{
		Normal,
		GeneralError,
		InvalidToken,
		InvalidCommandLineArgs
	}

	internal class Program
	{
		public delegate void InitializeEventHandler();

		/// <summary>
		/// Fired when the program starts.
		/// </summary>
		public static event InitializeEventHandler Initialized;

		public delegate void ExitEventHandler();

		/// <summary>
		/// Fired when the program begins exiting.
		/// </summary>
		public static event ExitEventHandler Exiting;

		private static void Main(string[] args)
		{
			var fileName = Assembly.GetExecutingAssembly().GetName().Name;
			var showHelp = false;

			var options = new OptionSet
			{
				{
					"n|name=", "The bot's name and default alias.",
					name => Bot.BotName = name
				},
				{
					"t|token=", "Required. The Slack API key to connect and operate with.",
					token => Bot.BotAccessToken = token ?? Environment.GetEnvironmentVariable("SLACK_API_TOKEN")
				},
				{
					"ut|usertoken=", "The Slack API key to use in certain operations such as delete.",
					token =>
						Bot.AccessToken =
							token ?? Environment.GetEnvironmentVariable("SLACK_API_USER_TOKEN") ?? Bot.BotAccessToken
				},
				{
					"h|help", "Show help about the command line arguments and exit.", h => showHelp = h != null
				}
			};

			try
			{
				options.Parse(args);
			}
			catch (OptionException ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine($"Please type {fileName} --help for more details.");
				Environment.Exit((int) ExitCodes.InvalidCommandLineArgs);
			}

			if (showHelp)
			{
				Console.WriteLine($"Usage: {fileName} --token <token> (options)");
				Console.WriteLine();

				// output the options
				Console.WriteLine("Options:");
				options.WriteOptionDescriptions(Console.Out);

				Environment.Exit((int) ExitCodes.Normal);
			}

			var tokenRegex = new Regex(@"^xo\w{2}-\d{11}-\w{24}", RegexOptions.IgnoreCase);

			if (string.IsNullOrEmpty(Bot.BotAccessToken) || !tokenRegex.IsMatch(Bot.BotAccessToken))
			{
				Console.WriteLine("A valid bot access token is required to start the program.");
				Environment.Exit((int) ExitCodes.InvalidToken);
			}

			if (string.IsNullOrEmpty(Bot.AccessToken) || !tokenRegex.IsMatch(Bot.AccessToken))
			{
				Console.WriteLine(
					"A valid user access token wasn't provided. Some features such as message deletion might not work.");
			}

			Initialized += EventHandlers.OnInitialize;
			Initialized?.Invoke();

			while (!ExitSignal)
			{
				var console = Console.ReadLine();

				if (string.IsNullOrWhiteSpace(console)) continue;

				var input = console.Split();

				var commandName = input[0].Replace("/", "");
				var parameters = input.Skip(1).ToArray();

				var command = CommandModule.Commands.FirstOrDefault(c => c.HasAlias(commandName));

				if (command == null)
				{
					Console.WriteLine("Invalid command entered! Type /help for a list of available commands!");
					continue;
				}

				command.Execute(commandName, parameters.ToList(), User.Self, new NewMessage {channel = "console"});

			}

			Exit();
		}

		internal static bool ExitSignal = false;

		private static void Exit(ExitCodes code = ExitCodes.Normal)
		{
			Exiting?.Invoke();
			Environment.Exit((int) code);
		}
	}
}