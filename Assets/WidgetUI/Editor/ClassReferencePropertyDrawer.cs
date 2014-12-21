using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using System.Linq;
using System.Collections.Generic;

namespace WidgetUI.Editor
{
	[CustomPropertyDrawer(typeof(ClassReference), true)]
	public class ClassReferencePropertyDrawer : PropertyDrawer
	{
		private struct TypeCache
		{
			public Type[] types;
			public String[] names;
		}

		// cache for reflection results
		Dictionary<Type, TypeCache> m_cache;

		public ClassReferencePropertyDrawer()
		{
			m_cache = new Dictionary<Type, TypeCache>();
		}


		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			// find the ClassReference object
			object targetObject = property.serializedObject.targetObject;
			string field = property.propertyPath;
			ClassReference reference = this.GetFieldValue(targetObject, field) as ClassReference;

			// get the types that inherit from the requested type and their names
			Type[] types;
			String[] names;
			this.GetValidTypesFor(reference.GetRequiredType(), out types, out names);

			// Find the index of the selected type in the array of available types or set the index to zero if not found.
			// No need to remember the index since Unity will use this PropertyDrawer multiple times for different fields.
			int selectedIndex = this.FindTypeIndex(types, reference.ReferencedClassType);
			if(selectedIndex < 0)
			{
				selectedIndex = 0;
				reference.ReferencedClassType = null;
			}

			// draw the label and the combobox
			label = EditorGUI.BeginProperty(position, label, property);
			Rect comboboxPosition = EditorGUI.PrefixLabel(position, label);

			int selected = EditorGUI.Popup(comboboxPosition, selectedIndex, names);
			if (selected != selectedIndex)
			{
				reference.ReferencedClassType = types[selected];
			}

			EditorGUI.EndProperty();
		}

		private object GetFieldValue(object p_object, string p_fieldName)
		{
			if (p_object == null)
			{
				return null;
			}

			Type type = p_object.GetType();

			// get the reflection info for the requested field
			FieldInfo fieldInfo = type.GetField(p_fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			if (fieldInfo == null)
			{
				// fallback: the field might not be a member but a property 
				PropertyInfo propertyInfo = type.GetProperty(p_fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
				if (propertyInfo == null)
				{
					return null; // give up
				}
				return propertyInfo.GetValue(p_object, null);
			}
			else
			{
				return fieldInfo.GetValue(p_object);
			}
		}

		private void GetValidTypesFor(Type p_type, out Type[] p_types, out String[] p_names)
		{
			TypeCache cachedValidTypes;

			// if there's no cache entry for the requested object, create it
			if(!m_cache.TryGetValue(p_type, out cachedValidTypes))
			{
				cachedValidTypes = this.GenerateTypeCache(p_type);
				m_cache.Add(p_type, cachedValidTypes);
			}

			p_types = cachedValidTypes.types;
			p_names = cachedValidTypes.names;
		}

		private TypeCache GenerateTypeCache(Type p_type)
		{
			TypeCache typeCache = new TypeCache();

			// find all classes that inherit from the requested type (using IsAssignableFrom()) using reflection
			var types = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(t => t.GetTypes())
				.Where(t => p_type.IsAssignableFrom(t) && t.IsClass)
				.OrderBy(t => t.FullName);

			// create a list of types from the result and prepend a "None" entry
			List<Type> typeList = types.ToList();
			typeList.Insert(0, null);
			typeCache.types = typeList.ToArray();

			// replace the period (namespace seperator) by a slash so that Unity will automatically create submenus for namespaces
			List<String> typeNameList = types.Select(t => this.GetTypeName(t)).ToList();
			typeNameList.Insert(0, String.Format("None ({0})", p_type.Name));
			typeCache.names = typeNameList.ToArray();

			return typeCache;
		}

		private string GetTypeName(Type p_type)
		{
			return p_type.FullName.Replace('.', '/');
		}

		private int FindTypeIndex(Type[] p_haystack, Type p_needle)
		{
			if (p_needle != null)
			{
				for (int i = 0; i < p_haystack.Length; ++i)
				{
					if (p_haystack[i] == p_needle)
					{
						return i;
					}
				}
			}

			return -1;
		}
	}
}
