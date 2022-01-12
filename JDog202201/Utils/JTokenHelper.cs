using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JDog202201.Utils
{
    public class JTokenHelper
    {
        public static string formatJtokenVal(JToken val, string def = "")
        {
            if (val != null)
            {
                def = val.ToString();
            }
            return def;
        }
    }
}
