using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JsonFx.Json;
namespace Nodeplay.UI.Utils
{

    public static class PrettyPrintWithJSON
    {
        public static string ToJSONstring(this object obj)
        {
            var serializer = new JsonWriter();
            var data = serializer.Write(obj);
            return data;
        }


        public static string ToJSONstring(this object obj, int recursiondepth)
        {

            var serialzer = new JsonWriter();
            serialzer.Settings.MaxDepth = recursiondepth;
            return serialzer.Write(obj);
        }
    }
}

