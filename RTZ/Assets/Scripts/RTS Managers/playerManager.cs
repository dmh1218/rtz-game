using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

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
				Directory.CreateDirectory ("SavedGames" + Path.DirectorySeparatorChar + name);
			}
						
			save ();
		}

		public static void save()
		{
			JsonSerializer serializer = new JsonSerializer ();
			serializer.NullValueHandling = NullValueHandling.Ignore;
			using (StreamWriter sw = new StreamWriter("SavedGames" + Path.DirectorySeparatorChar + "Players.json")) {
				using (JsonWriter writer = new JsonTextWriter(sw)) {
					writer.WriteStartObject ();
					writer.WritePropertyName ("Players");
					writer.WriteStartArray ();

					foreach (playerDetails player in players) {
						savePlayer (writer, player);
					}

					writer.WriteEndArray ();
					writer.WriteEndObject ();
				}
			}
		}

		public static void load()
		{
			players.Clear ();

			string filename = "SavedGames" + Path.DirectorySeparatorChar + "Players.json";
			if (File.Exists (filename)) {
				//read contents of file
				string input;
				using (StreamReader sr = new StreamReader(filename)) {
					input = sr.ReadToEnd ();
				}

				if (input != null) {
					//parse contents of file
					using (JsonTextReader reader = new JsonTextReader(new StringReader(input))) {
						while (reader.Read()) {
							if (reader.Value != null) {
								if (reader.TokenType == JsonToken.PropertyName) {
									if ((string)reader.Value == "Players") {
										loadPlayers (reader);
									} else {
									}
								}
							}
						}
					}
				}
			}
		}

		public static string getPlayerName()
		{
			return currentPlayer.Name == "" ? "Unknown" : currentPlayer.Name;
		}

		public static string[] getPlayerNames()
		{
			string[] playerNames = new string[players.Count];
			for (int i = 0; i < playerNames.Length; i++) {
				playerNames [i] = players [i].Name;
			}
			return playerNames;
		}

		private static void savePlayer(JsonWriter writer, playerDetails player)
		{
			writer.WriteStartObject ();

			writer.WritePropertyName ("Name");
			writer.WriteValue (player.Name);

			writer.WriteEndObject ();
		}

		private static void loadPlayers(JsonTextReader reader)
		{
			while (reader.Read()) {
				if (reader.TokenType == JsonToken.StartObject) {
					loadPlayer (reader);
				} else if (reader.TokenType == JsonToken.EndArray) {
					return;
				}
			}
		}

		private static void loadPlayer(JsonTextReader reader)
		{
			string currValue = "", name = "";

			while (reader.Read()) {
				if (reader.Value != null) {
					if (reader.TokenType == JsonToken.PropertyName) {
						currValue = (string)reader.Value;
					} else {
						switch (currValue) {
						case "Name":
							name = (string)reader.Value;
							break;
						default:
							break;
						}
					}
				} else {
					if (reader.TokenType == JsonToken.EndObject) {
						players.Add (new playerDetails (name));
						return;
					}
				}
			}
		}
	}
}
