using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Utils
{
    public class DemandBuilder
    {
        private IDictionary<string, object> props = new Dictionary<string, object>();
        private List<string> constraintRules = new List<string>();

        public IDictionary<string, object> Properties { get { return this.props; } }
        public string Constraints {
            get
            {
                switch(this.constraintRules.Count)
                {
                    case 0:
                        return "()";  // empty constraint
                    case 1:
                        return this.constraintRules[0]; // one constraint
                    default:
                        return $"(&{String.Join("\n\t", this.constraintRules)})";
                }
            }
        }

        /// <summary>
        /// Add constraint expression to the existing constraint set
        /// </summary>
        /// <param name="constraints"></param>
        public void Ensure(string constraint)
        {
            this.constraintRules.Add(constraint);
        }

        /// <summary>
        /// Add collection of properties to the demand.
        /// </summary>
        /// <param name="props"></param>
        public void Add(IDictionary<string, object> props)
        {
            foreach(var key in props.Keys)
            {
                this.props[key] = props[key];
            }
        }

        /// <summary>
        /// Add a property to the demand.
        /// </summary>
        /// <param name="props"></param>
        public void Add(string propName, object prop)
        {
            this.props[propName] = props[propName];
        }
    }
}
