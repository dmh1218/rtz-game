using UnityEngine;
using System.Collections.Generic;

namespace RTS
{
	public static class playerManager
	{
		private struct playerDetails
		{
			private string name;

			public playerDetails(string name)
			{
				this.name = name;
			}

			public string Name { get { return name; } }
		}

		private static List<playerDetails> players = new List<playerDetails>();
		private static playerDetails currentPlayer;

		public static void selectPlayer(string name)
		{
			//check player doesn't already exist
			bool playerExists = false;

			foreach (playerDetails player in players) {
				if (player.Name == name) {
					currentPlayer = player;
					playerExists = true;
				}
			}

			if (!playerExists) {
				playerDetails newPlayer = new playerDetails (name);
				players.Add (newPlayer);
				currentPlayer = newPlayer;
			}
		}

		public static string getPlayerName()
		{
			return currentPlayer.Name == "" ? "Unknown" : currentPlayer.Name;
		}
	}
}
