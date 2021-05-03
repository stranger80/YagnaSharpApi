using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using YagnaSharpApi.Utils;

namespace YagnaSharpApi.Examples
{
    class TurbogethPayload : IPackage
    {
        public async Task DecorateDemandAsync(DemandBuilder builder)
        {
            builder.Ensure($"{Properties.RUNTIME_NAME}={"turbogeth-managed"}");
            builder.Ensure($"{Properties.INF_MEM_GIB}>={16}");
            builder.Ensure($"{Properties.INF_STORAGE_GIB}>={1024}");
        }

        public Task<string> ResolveUrlAsync()
        {
            throw new NotImplementedException();
        }
    }
}
