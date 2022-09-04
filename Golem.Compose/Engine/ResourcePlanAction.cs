using Golem.Compose.Model.Gaom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Golem.Compose.Engine
{
    public class ResourcePlanAction
    {
        public enum ActionType 
        {
            Create,
            Update,
            Rebuild,
            Delete
        }

        public ActionType Action { get; set; }

        public Service Service { get; set; }

    }
}
