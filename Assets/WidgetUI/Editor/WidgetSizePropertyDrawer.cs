using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using System.Linq;
using System.Collections.Generic;

namespace WidgetUI.Editor
{
	[CustomPropertyDrawer(typeof(WidgetSize))]
	public class WidgetSizePropertyDrawer : PropertyDrawer
	{
		public WidgetSizePropertyDrawer()
		{
		}

		private void NextLine(ref Rect p_rect)
		{
			p_rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
		}

		private bool IsValueResizeMode(SerializedProperty p_property, string p_propertyPath)
		{
			SerializedProperty mode = p_property.FindPropertyRelative(p_propertyPath);
			return this.IsValueResizeMode(mode);
		}

		private bool IsValueResizeMode(SerializedProperty p_propertyField)
		{
			return (WidgetSize.ResizeMode)p_propertyField.enumValueIndex == WidgetSize.ResizeMode.Value;
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			const int lines = 3;
			return lines * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			SerializedProperty width = property.FindPropertyRelative("width");
			SerializedProperty height = property.FindPropertyRelative("height");
			SerializedProperty widthMode = property.FindPropertyRelative("widthMode");
			SerializedProperty heightMode = property.FindPropertyRelative("heightMode");

			Rect drawRect = position;
			drawRect.height = EditorGUIUtility.singleLineHeight;

			EditorGUI.LabelField(drawRect, label);
			NextLine(ref drawRect);
			DrawDimProperties(drawRect, width, widthMode);
			NextLine(ref drawRect);
			DrawDimProperties(drawRect, height, heightMode);
		}

		private void DrawDimProperties(Rect p_drawRect, SerializedProperty p_value, SerializedProperty p_mode)
		{
			++EditorGUI.indentLevel;
			Rect contentDrawRect = EditorGUI.PrefixLabel(p_drawRect, new GUIContent(p_value.displayName));
			--EditorGUI.indentLevel;

			contentDrawRect.width /= 2;
			EditorGUI.PropertyField(contentDrawRect, p_mode, GUIContent.none);
			contentDrawRect.x += contentDrawRect.width;

			EditorGUI.BeginDisabledGroup(!this.IsValueResizeMode(p_mode));
			EditorGUI.PropertyField(contentDrawRect, p_value, GUIContent.none);
			EditorGUI.EndDisabledGroup();
		}
	}
}
