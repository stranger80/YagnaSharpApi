using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golem.ActivityApi.Client.Model
{
    public class ExeScriptCommand
    {
        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public virtual string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

    }
}
