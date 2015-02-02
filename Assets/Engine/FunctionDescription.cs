using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;

namespace Nodeplay.Engine
{
	/// <summary>
	/// simple container for set of reflected information to point ZT node to
	/// </summary>
	public class FunctionDescription
	{
		public List<ParameterInfo> Parameters { get; set; }
		public MethodInfo MethodPointer{get;set;}
		public Type LoadedTypePointer { get; set; }

		public FunctionDescription(List<ParameterInfo> parameters, MethodInfo methodinfo, Type typepointer, Type nodetype)
		{
			Debug.Log("building a function description for: "+ typepointer.FullName + methodinfo.Name + parameters.ToString());
			Parameters = parameters;
			MethodPointer = methodinfo;
			LoadedTypePointer = typepointer;
		}
	}
}
