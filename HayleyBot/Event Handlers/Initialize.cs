using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Config;
using SlackAPI;

namespace HayleyBot
{
	internal static class EventHandlers
	{
		/// <summary>
		/// Gets invoked when the program starts.
		/// </summary>
		internal static void OnInitialize()
		{
			#region Event Registration

			Bot.ClientConnected += OnClientConnect;
			Program.Exiting += OnExit;

			#endregion

			XmlConfigurator.Configure();
			Bot.Connect();
		}

		internal static void OnExit()
		{
			Console.WriteLine("Exiting");
		}

		/// <summary>
		///   Gets invoked when bot successfully connected to the Slack server.
		/// </summary>
		internal static void OnClientConnect(LoginResponse response)
		{
			if (!response.ok)
			{
				Console.WriteLine("Connection failed to establish! Retrying...");
				Console.WriteLine(response.error);
				Bot.Connect();
			}
			else
			{
				Console.WriteLine("Connection Established!");
				Bot.Run();
			}
		}
	}
}