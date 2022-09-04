using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Golem.Compose.Engine
{
    public interface IResource<TModel>
    {
        /// <summary>
        /// Performs the read of current state of the resource, based on its current model state. 
        /// </summary>
        /// <param name="req"></param>
        /// <param name="resp"></param>
        void Read(ReadResourceRequest<TModel> req, ReadResourceResponse<TModel> resp);
        void Create(CreateResourceRequest<TModel> req, CreateResourceResponse<TModel> resp);
        void Update(UpdateResourceRequest<TModel> req, UpdateResourceResponse<TModel> resp);
        void Delete(DeleteResourceRequest<TModel> req, DeleteResourceResponse<TModel> resp);

    }
}
