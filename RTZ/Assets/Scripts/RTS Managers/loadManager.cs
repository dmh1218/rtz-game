using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace RTS 
{
	public static class loadManager
	{
		//public methods for loadmanager
		public static void loadGame(string filename)
		{
			char separator = Path.DirectorySeparatorChar;
			string path = "SavedGames" + separator + playerManager.getPlayerName () + separator + filename /*+ ".json"*/;

			if (!File.Exists (path)) {
				Debug.Log("Unable to Find " + path + ". Loading will crash, so aborting.");
				return;
			}

			string input;
			using (StreamReader sr = new StreamReader(path)) {
				input = sr.ReadToEnd();
			}

			if (input != null) {
				//parse contents of file
				using (JsonTextReader reader = new JsonTextReader (new StringReader(input))) {
					while (reader.Read()) {
						if (reader.Value != null) {
							if (reader.TokenType == JsonToken.PropertyName) {
								string property = (string)reader.Value;
								switch (property) {
									case "Sun":
										loadLighting(reader);
										break;
									case "Ground":
										loadTerrain(reader);
										break;
									case "Camera":
										loadCamera(reader);
										break;
									case "Resources":
										//loadResources(reader);
										break;
									case "Players":
										loadPlayers(reader);
										break;
									default:
										break;
								}
							}
						}
					}
				}
			}
		}

		public static Vector3 loadVector(JsonTextReader reader)
		{
			Vector3 position = new Vector3 (0, 0, 0);

			if (reader == null) {
				return position;
			}

			string currVal = "";
			while (reader.Read()) {
				if (reader.Value != null) {
					if (reader.TokenType == JsonToken.PropertyName) {
						currVal = (string)reader.Value;
					} else {
						switch (currVal) {
							case "x": 
								position.x = (float)loadManager.convertToFloat(reader.Value);
								break;
							case "y":
								position.y = (float)loadManager.convertToFloat(reader.Value);
								break;
							case "z":
								position.z = (float)loadManager.convertToFloat(reader.Value);
								break;
							default:
								break;
						}
					}
				} else if (reader.TokenType == JsonToken.EndObject) {
					return position;
				}
			}
			return position;
		}

		public static Quaternion loadQuaternion(JsonTextReader reader)
		{
			Quaternion rotation = new Quaternion (0, 0, 0, 0);

			if (reader == null) {
				return rotation;
			}

			string currVal = "";
			while (reader.Read()) {
				if (reader.Value != null) {
					if (reader.TokenType == JsonToken.PropertyName) {
						currVal = (string)reader.Value;
					} else {
						switch (currVal) {
						case "x":
							rotation.x = (float)loadManager.convertToFloat(reader.Value);
							break;
						case "y":
							rotation.y = (float)loadManager.convertToFloat(reader.Value);
							break;
						case "z":
							rotation.z = (float)loadManager.convertToFloat(reader.Value);
							break;
						case "w":
							rotation.w = (float)loadManager.convertToFloat(reader.Value);
							break;
						default:
							break;
						}
					}
				} else if (reader.TokenType == JsonToken.EndObject) {
					return rotation;
				}
			}
			return rotation;
		}

		public static Color loadColor(JsonTextReader reader)
		{
			Color color = new Color (0, 0, 0, 0);
			if (reader == null) {
				return color;
			}

			string currVal = "";

			while (reader.Read()) {
				if (reader.Value != null) {
					if (reader.TokenType == JsonToken.PropertyName) {
						currVal = (string)reader.Value;
					} else {
						switch (currVal) {
						case "r":
							color.r = (float)loadManager.convertToFloat(reader.Value);
							break;
						case "g":
							color.g = (float)loadManager.convertToFloat(reader.Value);
							break;
						case "b":
							color.b = (float)loadManager.convertToFloat(reader.Value);
							break;
						case "a":
							color.a = (float)loadManager.convertToFloat(reader.Value);
							break;
						default:
							break;
						}
					}
				} else if (reader.TokenType == JsonToken.EndObject) {
					return color;
				}
			}

			return color;
		}

		public static Rect loadRect(JsonTextReader reader)
		{
			Rect rect = new Rect (0, 0, 0, 0);

			if (reader == null) {
				return rect;
			}

			string currValue = "";

			while (reader.Read()) {
				if (reader.Value != null) {
					if (reader.TokenType == JsonToken.PropertyName) {
						currValue = (string)reader.Value;
					} else {
						switch (currValue) {
						case "x":
							rect.x = (float)loadManager.convertToFloat(reader.Value);
							break;
						case "y":
							rect.y = (float)loadManager.convertToFloat(reader.Value);
							break;
						case "width":
							rect.width = (float)loadManager.convertToFloat(reader.Value);
							break;
						case "height":
							rect.height = (float)loadManager.convertToFloat(reader.Value);
							break;
						default:
							break;
						}
					}
				} else if (reader.TokenType == JsonToken.EndObject) {
					return rect;
				}
			}
			return rect;
		}

		public static List<string> loadStringArray(JsonTextReader reader)
		{
			List<string> values = new List<string>();
			while (reader.Read()) {
				if (reader.Value != null) {
					values.Add ((string)reader.Value);
				} else if (reader.TokenType == JsonToken.EndArray) {
					return values;
				}
			} 
			return values;
		}

		public static float convertToFloat(object value)
		{
			float val1 = new float ();
			if (value.GetType () == typeof(long)) {
				val1 = (float)(long)value;
			}
			if (value.GetType () == typeof(double)) {
				val1 = (float)(double)value;
			}
			return val1;
		}

		//private methods for loadmanager
		private static void loadLighting(JsonTextReader reader)
		{
			if (reader == null) {
				return;
			}

			Vector3 position = new Vector3 (0, 0, 0);
			Vector3 scale = new Vector3 (1, 1, 1);
			Quaternion rotation = new Quaternion (0, 0, 0, 0);

			while (reader.Read()) {
				if (reader.Value != null) {
					if (reader.TokenType == JsonToken.PropertyName) {
						if ((string)reader.Value == "Position") {
							position = loadVector(reader);
						} else if ((string)reader.Value == "Rotation") {
							rotation = loadQuaternion(reader);
						} else if ((string)reader.Value == "Scale") {
							scale = loadVector(reader);
						}
					}
				} else if (reader.TokenType == JsonToken.EndObject) {
					GameObject sun = (GameObject)GameObject.Instantiate(resourceManager.getWorldObject("Sun"), position, rotation);
					sun.transform.localScale = scale;
					return;
				}
			}

		}

		private static void loadTerrain(JsonTextReader reader)
		{
			if (reader == null) {
				return;
			}

			Vector3 position = new Vector3 (0, 0, 0);
			Vector3 scale = new Vector3 (1, 1, 1);
			Quaternion rotation = new Quaternion (0, 0, 0, 0);
			while (reader.Read()) {
				if (reader.Value != null) {
					if (reader.TokenType == JsonToken.PropertyName) {
						if ((string)reader.Value == "Position") {
							position = loadVector (reader);
						} else if ((string)reader.Value == "Rotation") {
							rotation = loadQuaternion (reader);
						} else if ((string)reader.Value == "Scale") {
							scale = loadVector (reader);
						}
					}
				} else if (reader.TokenType == JsonToken.EndObject) {
					GameObject ground = (GameObject)GameObject.Instantiate (resourceManager.getWorldObject ("Ground"), position, rotation);
					ground.transform.localScale = scale;
					return;
				}
			}
		}

		private static void loadCamera(JsonTextReader reader)
		{
			if (reader == null) {
				return;
			}

			Vector3 position = new Vector3 (0, 0, 0);
			Vector3 scale = new Vector3 (1, 1, 1);
			Quaternion rotation = new Quaternion (0, 0, 0, 0);

			while (reader.Read()) {
				if (reader.Value != null) {
					if (reader.TokenType == JsonToken.PropertyName) {
						if ((string)reader.Value == "Position") {
							position = loadVector (reader);
						} else if ((string)reader.Value == "Rotation") {
							rotation = loadQuaternion (reader);
						} else if ((string)reader.Value == "Scale") {
							scale = loadVector (reader);
						}
					}
				} else if (reader.TokenType == JsonToken.EndObject) {
					GameObject camera = Camera.main.gameObject;
					camera.transform.localPosition = position;
					camera.transform.localRotation = rotation;
					camera.transform.localScale = scale;

					return;
				}
			}
		}

		private static void loadResources(JsonTextReader reader)
		{
			if (reader == null) {
				return;
			}

			string currValue = "";
			string type = "";

			while (reader.Read()) {
				if (reader.Value != null) {
					if (reader.TokenType == JsonToken.PropertyName) {
						currValue = (string)reader.Value;
					} else if (currValue == "Type") {
						type = (string)reader.Value;
						Debug.Log(type);
						GameObject newObject = (GameObject)GameObject.Instantiate (resourceManager.getWorldObject (type));
						Resource resource = newObject.GetComponent<Resource> ();
						resource.loadDetails (reader);
					}
				} else if (reader.TokenType == JsonToken.EndArray) {
					return;
				}
			}
		}

		private static void loadPlayers(JsonTextReader reader)
		{
			int x = 0;
			if (reader == null) {
				return;
			}
			Debug.Log ("load players");

			while (reader.Read()) {
				if (reader.TokenType == JsonToken.StartObject) {
					x++;
					//Debug.Log ("start object " + x);
					GameObject newObject = (GameObject)GameObject.Instantiate (resourceManager.getPlayerObject ());
					Player player = newObject.GetComponent<Player> ();
					player.loadDetails (reader);
				} else if (reader.TokenType == JsonToken.EndArray) {
					return;
				}
			}
		}

	}


}