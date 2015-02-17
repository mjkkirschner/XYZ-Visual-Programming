
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using jsonfx =  Pathfinding.Serialization.JsonFx;

namespace Nodeplay.Core
{
	
		
	public class VariableReference:jsonfx.JsonConverter
	{	[JsonIgnore]
		public Func<object> Get { get; private set; }
		[JsonIgnoreAttribute]
		public Action<object> Set { get;  private set; }

		public VariableReference(Func<object> getter, Action<object> setter)
		{
			Get = getter;
			Set = setter;
		}
	}
}
