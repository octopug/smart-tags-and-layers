using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SmartTagsAndLayers
{
	[InitializeOnLoad]
	public class TagsAndLayersGenerator
	{
		#region Settings Variables

		private static bool autoGenerate = true;
		private static float generateBuffer = 1f;
		private static string scriptFolderPathAbsolute = Application.dataPath + "/Extensions/SmartTagsAndLayers/Scripts/";

		#endregion

		#region Variables

		private static bool generateLayersWait = false;
		private static double generateLayersAt = 0f;
		private static string[] layersBuffer = new string[0];

		private static bool generateTagsWait = false;
		private static double generateTagsAt = 0f;
		private static string[] tagsBuffer = new string[0];

		#endregion

		#region Constructor

		static TagsAndLayersGenerator()
		{
			if (!EditorApplication.isPlayingOrWillChangePlaymode)
			{
				EditorApplication.update += Update;

				// Load buffers if classes are the same
				if (Layers.List.Count == UnityEditorInternal.InternalEditorUtility.layers.Length)
				{
					bool loadLayersBuffer = true;
					for (int i = 0; i < Layers.List.Count; i++)
					{
						if (Layers.List[i].Name != UnityEditorInternal.InternalEditorUtility.layers[i])
						{
							loadLayersBuffer = false;
							break;
						}
					}
					if (loadLayersBuffer)
						layersBuffer = UnityEditorInternal.InternalEditorUtility.layers;
				}
				if (Tags.List.Count == UnityEditorInternal.InternalEditorUtility.tags.Length)
				{
					bool loadTagsBuffer = true;
					for (int i = 0; i < Tags.List.Count; i++)
					{
						if (Tags.List[i] != UnityEditorInternal.InternalEditorUtility.tags[i])
						{
							loadTagsBuffer = false;
							break;
						}
					}
					if (loadTagsBuffer)
						tagsBuffer = UnityEditorInternal.InternalEditorUtility.tags;
				}
			}
		}

		#endregion

		#region Update

		private static void Update()
		{
			// Layers
			if (autoGenerate && !Enumerable.SequenceEqual(layersBuffer, UnityEditorInternal.InternalEditorUtility.layers))
			{
				layersBuffer = UnityEditorInternal.InternalEditorUtility.layers;
				generateLayersWait = true;
				generateLayersAt = EditorApplication.timeSinceStartup + generateBuffer;
			}
			if (generateLayersWait && generateLayersAt < EditorApplication.timeSinceStartup)
			{
				generateLayersWait = false;
				GenerateLayers();
			}

			// Tags
			if (autoGenerate && !Enumerable.SequenceEqual(tagsBuffer, UnityEditorInternal.InternalEditorUtility.tags))
			{
				tagsBuffer = UnityEditorInternal.InternalEditorUtility.tags;
				generateTagsWait = true;
				generateTagsAt = EditorApplication.timeSinceStartup + generateBuffer;
			}
			if (generateTagsWait && generateTagsAt < EditorApplication.timeSinceStartup)
			{
				generateTagsWait = false;
				GenerateTags();
			}
		}

		#endregion

		#region Menu Items

		[MenuItem("Tools/Smart Tags and Layers/Generate Layers", false, 0)]
		public static void MenuItemGenerateLayers()
		{
			GenerateLayers();
		}

		[MenuItem("Tools/Smart Tags and Layers/Generate Tags", false, 0)]
		public static void MenuItemGenerateTags()
		{
			GenerateTags();
		}

		#endregion

		#region Layer Methods

		private static void GenerateLayers()
		{
			// Create folders if necessary
			if (!Directory.Exists(scriptFolderPathAbsolute))
				Directory.CreateDirectory(scriptFolderPathAbsolute);

			// Create the file
			File.WriteAllText(scriptFolderPathAbsolute + "Layers.cs", GenerateLayersFile());

			// Refresh asset database
			AssetDatabase.Refresh();
		}

		private static string GenerateLayersFile()
		{
			string output = "";

			output += "// This class is auto-generated DO NOT MODIFY\n";
			output += "// Use Tools >> Smart Tags and Layers >> Generate Layers on the menu to update this class\n";
			output += "\n";
			output += "using System.Collections.Generic;\n";
			output += "\n";
			output += "namespace SmartTagsAndLayers\n";
			output += "{\n";
			output += "\tpublic static class Layers\n";
			output += "\t{\n";
			output += "\t\t#region Variables\n";
			output += "\t\t\n";

			Dictionary<string, string> layers = BuildFormattedValuesDictionary(UnityEditorInternal.InternalEditorUtility.layers);
			foreach (var layerData in layers)
			{
				output += "\t\tpublic static Layer " + layerData.Key + " = new Layer(\"" + layerData.Value + "\");\n";
			}

			output += "\t\t\n";
			output += "\t\t#endregion\n";
			output += "\t\n";
			output += "\t\t#region List\n";
			output += "\t\t\n";
			output += "\t\tpublic static List<Layer> List = new List<Layer>()\n";
			output += "\t\t{\n";

			foreach (var varName in layers.Keys)
			{
				output += "\t\t\t" + varName + ",\n";
			}

			output += "\t\t};\n";
			output += "\t\t\n";
			output += "\t\t#endregion\n";
			output += "\t}\n";
			output += "}";

			return output;
		}

		#endregion

		#region Tag Methods

		private static void GenerateTags()
		{
			// Create folders if necessary
			if (!Directory.Exists(scriptFolderPathAbsolute))
				Directory.CreateDirectory(scriptFolderPathAbsolute);

			// Create the file
			File.WriteAllText(scriptFolderPathAbsolute + "Tags.cs", GenerateTagsFile());

			// Refresh asset database
			AssetDatabase.Refresh();
		}

		private static string GenerateTagsFile()
		{
			string output = "";

			output += "// This class is auto-generated DO NOT MODIFY\n";
			output += "// Use Tools >> Smart Tags and Layers >> Generate Tags on the menu to update this class\n";
			output += "\n";
			output += "using System.Collections.Generic;\n";
			output += "\n";
			output += "namespace SmartTagsAndLayers\n";
			output += "{\n";
			output += "\tpublic static class Tags\n";
			output += "\t{\n";
			output += "\t\t#region Variables\n";
			output += "\t\t\n";

			Dictionary<string, string> tags = BuildFormattedValuesDictionary(UnityEditorInternal.InternalEditorUtility.tags);
			foreach (var tagData in tags)
			{
				output += "\t\tpublic const string " + tagData.Key + " = \"" + tagData.Value + "\";\n";
			}

			output += "\t\t\n";
			output += "\t\t#endregion\n";
			output += "\t\n";
			output += "\t\t#region List\n";
			output += "\t\t\n";
			output += "\t\tpublic static List<string> List = new List<string>()\n";
			output += "\t\t{\n";

			foreach (var varName in tags.Keys)
			{
				output += "\t\t\t" + varName + ",\n";
			}

			output += "\t\t};\n";
			output += "\t\t\n";
			output += "\t\t#endregion\n";
			output += "\t}\n";
			output += "}";

			return output;
		}

		#endregion

		#region String Helpers

		private static string FormatForVariableName(string _value)
		{
			string output = "";
			bool nextUpper = true;
			bool allowNumber = false;

			for (int i = 0; i < _value.Length; i++)
			{
				bool nextUpperBuffer = nextUpper;
				nextUpper = false;

				char c = _value[i];
				if (!IsValidCharacterInVariableName(c, allowNumber))
				{
					nextUpper = true;
					continue;
				}

				allowNumber = true;

				if (nextUpperBuffer)
					output += Char.ToUpper(c);
				else
					output += c;
			}

			return output;
		}

		private static bool IsValidCharacterInVariableName(char c, bool allowNumber)
		{
			return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (allowNumber && c >= '0' && c <= '9');
		}

		private static string FormatForValue(string _value)
		{
			return _value
				.Replace("\\", "\\\\") // Replace \ with \\
				.Replace("\"", "\\\"") // Replace " with \"
				.Replace("\'", "\\\'"); // Replace ' with \'
		}

		private static Dictionary<string, string> BuildFormattedValuesDictionary(string[] array)
		{
			var dict = new Dictionary<string, string>();
			for (int i = 0; i < array.Length; i++)
			{
				string raw = array[i];
				string varName = FormatForVariableName(raw);
				if (string.IsNullOrEmpty(varName))
				{
					Debug.LogError("Variable name generated for tag/layer named '" + raw + "' was null or empty! Is it all invalid characters or numbers?");
					continue;
				}
				else if (dict.ContainsKey(varName))
				{
					Debug.LogError("Conflict found with tag/layer named '" + raw + "', check files for missing entries!");
					continue;
				}

				string value = FormatForValue(raw);
				dict.Add(varName, value);
			}
			return dict;
		}

		#endregion
	}
}
