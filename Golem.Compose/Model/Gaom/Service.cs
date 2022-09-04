using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Golem.Compose.Model.Gaom
{
    public class Service
    {
        public string Name { get; set; }

        /// <summary>
        /// State enum - indicates the state of the service
        /// </summary>
        public ResourceState State { get; set; }
        
        /// <summary>
        /// Actual AgreementId which covers the service instance
        /// </summary>
        public string AgreementId { get; set; }

        /// <summary>
        /// Actual ActivityId which covers the service instance
        /// </summary>
        public string ActivityId { get; set; }

        public Payload Payload { get; set; }
        public Network Network { get; set; }

        public IList<Service> DependsOn { get; set; }

        public IList<ExeScriptCommand> Entrypoint { get; set; } = new List<ExeScriptCommand>();





        
    }
}
