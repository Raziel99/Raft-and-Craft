using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ArmNomads.Haptic;

namespace ArmNomads.Haptic
{
	[CustomEditor(typeof(HapticManager))]
	public class HapticManagerEditor : Editor
	{
		private static readonly string HAPTIC_CONFIG_INFO_STRING_FORMAT = "Light: {0}ms,\nMedium: {1}ms,\nHeavy: {2}ms";

		private SerializedProperty hapticConfigProperty;

		private string hapticConfigString = "";
		private string hapticConfigInfoString;

		private void OnEnable()
		{
			hapticConfigProperty = serializedObject.FindProperty("hapticConfig");
			UpdateHapticConfigInfoString();
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			serializedObject.Update();
			EditorGUILayout.LabelField("Haptic Config String", EditorStyles.boldLabel);
			hapticConfigString = EditorGUILayout.TextArea(hapticConfigString, GUILayout.MaxHeight(50));
			if (GUILayout.Button("Apply!"))
			{
				ParseHapticConfigString();
				UpdateHapticConfigInfoString();
				hapticConfigString = "";
			}
			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.PrefixLabel("Applied Settings");
				EditorGUILayout.HelpBox(hapticConfigInfoString, MessageType.Info);
			}
			EditorGUILayout.EndHorizontal();
			serializedObject.ApplyModifiedProperties();
		}

		private void ParseHapticConfigString()
		{
			string[] configLines = hapticConfigString.Split('\n');
			if (configLines.Length < hapticConfigProperty.arraySize)
				return;
			int[] parsedValues = new int[hapticConfigProperty.arraySize];
			for (int i = 0; i < parsedValues.Length; ++i)
			{
				if (!int.TryParse(configLines[i], out parsedValues[i]))
					return;
			}
			for (int i = 0; i < parsedValues.Length; ++i)
			{
				hapticConfigProperty.GetArrayElementAtIndex(i).intValue = parsedValues[i];
			}

		}

		private void UpdateHapticConfigInfoString()
		{
			hapticConfigInfoString = string.Format(HAPTIC_CONFIG_INFO_STRING_FORMAT, hapticConfigProperty.GetArrayElementAtIndex(0).intValue,
																					 hapticConfigProperty.GetArrayElementAtIndex(1).intValue,
																					 hapticConfigProperty.GetArrayElementAtIndex(2).intValue);
		}
	}
}
