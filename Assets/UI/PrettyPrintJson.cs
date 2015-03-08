using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using jsonfx =  Pathfinding.Serialization.JsonFx;
using UnityEngine;
using Nodeplay.Core;
namespace Nodeplay.UI.Utils
{

    public static class PrettyPrintWithJSON
    {
        public static string ToJSONstring(this object obj)
        {
			var settings = new jsonfx.JsonWriterSettings();
			//settings.PrettyPrint = true;
					
			System.Text.StringBuilder output = new System.Text.StringBuilder();
			var writer = new jsonfx.JsonWriter (output,settings);

			//writer.Settings.HandleCyclicReferences = true;
			if (obj.GetType() == typeof(VariableReference))
			{
				Debug.Log( ((VariableReference)obj).VariableName);
			}
            writer.Write(obj);
            return output.ToString();
        }


        public static string ToJSONstring(this object obj, int recursiondepth)
        {

			var settings = new jsonfx.JsonWriterSettings();
			//settings.PrettyPrint = true;
			
			System.Text.StringBuilder output = new System.Text.StringBuilder();
			var writer = new jsonfx.JsonWriter (output,settings);
			
			writer.Settings.HandleCyclicReferences = true;
			writer.Settings.MaxDepth = recursiondepth;
			writer.Write(obj);
			return output.ToString();

        }
    }
}

