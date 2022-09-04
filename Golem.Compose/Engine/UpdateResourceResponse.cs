using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Golem.Compose.Engine
{
    public class UpdateResourceResponse<TModel>
    {
        public TModel State { get; set; }
    }
}
