using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Golem.Compose.Engine
{
    public class Engine
    {
        /// <summary>
        /// Create resource provisioning plan, based on the descriptor (desired state), persisted state and actual state.
        /// </summary>
        /// <param name="descriptor"></param>
        /// <param name="state"></param>
        public IEnumerable<ResourcePlanAction> Plan(Model.Gaom.Application descriptor, Model.Gaom.Application state)
        {
            var result =new List<ResourcePlanAction>();

            // 1. Reconcile persisted state vs actual state
            // - for each resource in persisted state - read its current model and update (this may cause resources to disappear from the persisted state!)

            foreach(var serviceModel in state.Services)
            {
                var res = new ServiceResource();

                
            }


            // 2. Reconcile updated state vs desired state (descriptor)
            // - for each resource that is in desired but not in updated - add Create to the action list
            // - for each resource that is in desired and in updated and has changed - check if update can be perfomed or a recreate is required
            //   - if update can be performed - add Update to the action list
            //   - if update cannot be performed - add Rebuild to the action list
            // - for each resource that is in updated but not in desired - add Delete to the action list

            // 3. Determine knock-on effect of action
            // - for each resource Deleted - no need to determine the knock-on effect, as there must not be any depending resources (otherwise the descriptor would be incosistent)
            // - for each resource Updated - for now: don't do anything 
            // - for each resource Rebuilt - check dependent resources and add their Rebuild to the action list
            //   (NOTE: this is overkill, but more accurate knock-on effect can only be determined if we assess individual attributes&references:
            //   eg. we know that after Rebuild an IP address of a VM changes, so we only need to update/rebuild those dependent resources,
            //   which refer to the IP address attribute) 



            return result;
        }

    }
}
