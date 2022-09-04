using Golem.Compose.Model.Gaom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Golem.Compose.Engine
{
    public class ServiceResource : IResource<Service>
    {
        public void Create(CreateResourceRequest<Service> req, CreateResourceResponse<Service> resp)
        {
            throw new NotImplementedException();
        }

        public void Delete(DeleteResourceRequest<Service> req, DeleteResourceResponse<Service> resp)
        {
            throw new NotImplementedException();
        }

        public void Read(ReadResourceRequest<Service> req, ReadResourceResponse<Service> resp)
        {
            var resourceModel = req.State;

            // now verify the actual state of the Service - by checking the state of corresponding Agreement and Activity



            throw new NotImplementedException();
        }

        public void Update(UpdateResourceRequest<Service> req, UpdateResourceResponse<Service> resp)
        {
            throw new NotImplementedException();
        }
    }
}
