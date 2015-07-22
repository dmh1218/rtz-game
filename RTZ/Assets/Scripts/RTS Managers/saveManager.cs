using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace RTS
{
	public static class saveManager 
	{

		//public methods

		public static void saveGame(string filename)
		{
			JsonSerializer serializer = new JsonSerializer ();
			serializer.NullValueHandling = NullValueHandling.Ignore;
			Directory.CreateDirectory ("SavedGames");
			char separator = Path.DirectorySeparatorChar;
			string path = "SavedGames" + separator + playerManager.getPlayerName () + separator + filename /*+ ".json"*/;
			using (StreamWriter sw = new StreamWriter(path)) {
				using (JsonWriter writer = new JsonTextWriter(sw)) {
					writer.WriteStartObject ();
					saveGameDetails (writer);
					writer.WriteEndObject ();
				}
			}
		}

		public static void writeVector(JsonWriter writer, string name, Vector3 vector)
		{
			if (writer == null) {
				return;
			}

			writer.WritePropertyName (name);
			writer.WriteStartObject ();
			writer.WritePropertyName ("x");
			writer.WriteValue (vector.x);
			writer.WritePropertyName ("y");
			writer.WriteValue (vector.y);
			writer.WritePropertyName ("z");
			writer.WriteValue (vector.z);
			writer.WriteEndObject ();
		}

		public static void writeQuaternion(JsonWriter writer, string name, Quaternion quaternion)
		{
			if (writer == null) {
				return;
			}

			writer.WritePropertyName (name);
			writer.WriteStartObject ();
			writer.WritePropertyName ("x");
			writer.WriteValue (quaternion.x);
			writer.WritePropertyName ("y");
			writer.WriteValue (quaternion.y);
			writer.WritePropertyName ("z");
			writer.WriteValue (quaternion.z);
			writer.WritePropertyName ("w");
			writer.WriteValue (quaternion.w);
			writer.WriteEndObject ();
		}

		public static void writeString(JsonWriter writer, string name, string entry)
		{
			if (writer == null) {
				return;
			}

			writer.WritePropertyName (name);
			//make sure no bracketed values get stored (e.g. Zombie(Clone) becomes Zombie)
			if (entry.Contains ("(")) {
				writer.WriteValue (entry.Substring (0, entry.IndexOf ("(")));
			} else {
				writer.WriteValue (entry);
			}

		}

		public static void writeInt(JsonWriter writer, string name, int amount)
		{
			if (writer == null) {
				return;
			}

			writer.WritePropertyName (name);
			writer.WriteValue (amount);
		}

		public static void writeFloat(JsonWriter writer, string name, float amount)
		{
			if (writer == null) {
				return;
			}

			writer.WritePropertyName (name);
			writer.WriteValue (amount);
		}

		public static void writeBoolean(JsonWriter writer, string name, bool state)
		{
			if (writer == null) {
				return;
			}

			writer.WritePropertyName (name);
			writer.WriteValue (state);
		}

		public static void writeColor(JsonWriter writer, string name, Color color)
		{
			if (writer == null) {
				return;
			}

			writer.WritePropertyName (name);
			writer.WriteStartObject ();
			writer.WritePropertyName ("r");
			writer.WriteValue (color.r);
			writer.WritePropertyName ("g");
			writer.WriteValue (color.g);
			writer.WritePropertyName ("b");
			writer.WriteValue (color.b);
			writer.WritePropertyName ("a");
			writer.WriteValue (color.a);
			writer.WriteEndObject ();
		}

		public static void writeStringArray(JsonWriter writer, string name, string[] values)
		{
			if (writer == null) {
				return;
			}

			writer.WritePropertyName (name);
			writer.WriteStartArray ();
			foreach (string v in values) {
				writer.WriteValue (v);
			}
			writer.WriteEndArray ();
		}

		public static void saveWorldObject(JsonWriter writer, WorldObject worldObject)
		{
			if (writer == null || worldObject == null) {
				return;
			}

			writer.WriteStartObject ();
			worldObject.saveDetails (writer);
			writer.WriteEndObject ();
		}

		public static void savePlayerResources(JsonWriter writer, Dictionary<resourceType, int> resources, Dictionary<resourceType, int>resourceLimits)
		{
			if (writer == null) {
				return;
			}

			writer.WritePropertyName ("Resources");
			writer.WriteStartArray ();

			foreach (KeyValuePair<resourceType, int> pair in resources) {
				writer.WriteStartObject();
				writeInt (writer, pair.Key.ToString (), pair.Value);
				writer.WriteEndObject();
			}
			foreach (KeyValuePair<resourceType, int>pair in resourceLimits) {
				writer.WriteStartObject();
				writeInt(writer, pair.Key.ToString() + "_Limit", pair.Value);
				writer.WriteEndObject();
			}
			writer.WriteEndArray ();
		}

		public static void savePlayerBuildings(JsonWriter writer, Building[] buildings)
		{
			if (writer == null) {
				return;
			}

			writer.WritePropertyName ("Buildings");
			writer.WriteStartArray ();
			foreach (Building building in buildings) {
				saveWorldObject (writer, building);
			}
			writer.WriteEndArray ();
		}

		public static void savePlayerUnits(JsonWriter writer, Unit[] units)
		{
			if (writer == null) {
				return;
			}

			writer.WritePropertyName ("Units");
			writer.WriteStartArray ();
			foreach (Unit unit in units) {
				saveWorldObject (writer, unit);
			}
			writer.WriteEndArray ();
		}

		public static void writeRect(JsonWriter writer, string name, Rect rect)
		{
			if (writer == null) {
				return;
			}

			writer.WritePropertyName (name);
			writer.WriteStartObject ();
			writer.WritePropertyName ("x");
			writer.WriteValue (rect.x);
			writer.WritePropertyName ("y");
			writer.WriteValue (rect.y);
			writer.WritePropertyName ("width");
			writer.WriteValue (rect.width);
			writer.WritePropertyName ("height");
			writer.WriteValue (rect.height);
			writer.WriteEndObject ();
		}

		//private methods

		private static void saveGameDetails(JsonWriter writer)
		{
			saveLighting (writer);
			saveTerrain (writer);
			saveCamera (writer);
			saveResources (writer);
			savePlayers (writer);
		}

		private static void saveLighting(JsonWriter writer)
		{
			Sun sun = (Sun)GameObject.FindObjectOfType (typeof(Sun));

			if (writer == null || sun == null) {
				return;
			}

			writer.WritePropertyName ("Sun");
			writer.WriteStartObject ();

			writeVector (writer, "Position", sun.transform.position);
			writeQuaternion (writer, "Rotation", sun.transform.rotation);
			writeVector (writer, "Scale", sun.transform.localScale);

			writer.WriteEndObject ();
		}

		private static void saveTerrain(JsonWriter writer)
		{
			//needs to be adapted for terrain once if that gets implemented
			Ground ground = (Ground)GameObject.FindObjectOfType (typeof(Ground));

			if (writer == null || ground == null) {
				return;
			}

			writer.WritePropertyName ("Ground");
			writer.WriteStartObject ();

			writeVector (writer, "Position", ground.transform.position);
			writeQuaternion (writer, "Rotation", ground.transform.rotation);
			writeVector (writer, "Scale", ground.transform.localScale);

			writer.WriteEndObject ();
		}

		private static void saveCamera(JsonWriter writer)
		{
			if (writer == null) {
				return;
			}

			writer.WritePropertyName ("Camera");
			writer.WriteStartObject ();

			Transform cameraTransform = Camera.main.transform;
			writeVector (writer, "Position", cameraTransform.position);
			writeQuaternion (writer, "Rotation", cameraTransform.rotation);
			writeVector (writer, "Scale", cameraTransform.localScale);

			writer.WriteEndObject ();
		}

		private static void saveResources(JsonWriter writer)
		{
			Resource[] resources = GameObject.FindObjectsOfType (typeof(Resource)) as Resource[];

			if (writer == null || resources == null) {
				return;
			}

			writer.WritePropertyName ("Resources");
			writer.WriteStartArray ();

			foreach (Resource resource in resources) {
				saveWorldObject (writer, resource);
			}

			writer.WriteEndArray ();
		}

		private static void savePlayers(JsonWriter writer)
		{
			Player[] players = GameObject.FindObjectsOfType (typeof(Player)) as Player[];

			if (writer == null || players == null) {
				return;
			}

			writer.WritePropertyName ("Players");
			writer.WriteStartArray ();

			foreach (Player player in players) {
				writer.WriteStartObject ();
				Debug.Log("start new player object");

				player.saveDetails (writer);
				writer.WriteEndObject ();
			}

			writer.WriteEndArray ();
		}

	}
}
