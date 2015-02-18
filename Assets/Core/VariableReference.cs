
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using jsonfx =  Pathfinding.Serialization.JsonFx;

namespace Nodeplay.Core
{
	
	[jsonfx.JsonOptIn]	
	public class VariableReference
	{	
		[jsonfx.JsonMember]
		public Func<object> Get { get; private set; }
		public Action<object> Set { get;  private set; }
		[jsonfx.JsonMember]
		public string VariableName {get;set;}

		public VariableReference(Func<object> getter, Action<object> setter,string variablename)
		{
			Get = getter;
			Set = setter;
			VariableName = variablename;
		}
		

	}
}
