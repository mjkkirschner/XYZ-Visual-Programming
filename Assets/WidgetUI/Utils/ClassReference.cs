using System;
using UnityEngine;

namespace WidgetUI
{
	public abstract class ClassReference
	{
		[SerializeField]
		private String m_serializedClassType = String.Empty;
		private Type m_referencedClass = null;

		public Type ReferencedClassType
		{
			get
			{
				if (m_referencedClass == null && !String.IsNullOrEmpty(m_serializedClassType))
				{
					m_referencedClass = Type.GetType(m_serializedClassType);
				}
				return m_referencedClass;
			}
			set
			{
				m_referencedClass = value;
				m_serializedClassType = (value == null) ? String.Empty : value.AssemblyQualifiedName;
			}
		}

		public abstract Type GetRequiredType();
	}


	public abstract class ClassReference<T> : ClassReference
	{
		public override Type GetRequiredType()
		{
			return typeof(T);
		} 
	}

}
