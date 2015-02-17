using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using jsonfx =  Pathfinding.Serialization.JsonFx;
namespace Nodeplay.UI.Utils
{

    public static class PrettyPrintWithJSON
    {
        public static string ToJSONstring(this object obj)
        {

			var serializer = new jsonfx.JsonWriter();
			serializer.Settings.GraphCycles = JsonFx.Serialization.GraphCycles.GraphCycleType.Reference;
            var data = serializer.Write(obj);
            return data;
        }


        public static string ToJSONstring(this object obj, int recursiondepth)
        {
			var serializer = new jsonfx.JsonWriter();
            serializer.Settings.MaxDepth = recursiondepth;
			serializer.Settings.GraphCycles = JsonFx.Serialization.GraphCycles.GraphCycleType.MaxDepth;
            return serializer.Write(obj);
        }
    }
}

