using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace YagnaSharpApi.Utils
{
    public interface IPackage
    {
        Task<string> ResolveUrlAsync();
        Task DecorateDemandAsync(DemandBuilder builder);
    }
}
