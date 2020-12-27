using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Utils
{
    public struct VmConstraints
    {
        public decimal MinMemGiB { get; set; }
        public decimal MinStorageGiB { get; set; }
        public int Cores { get; set; }

        public VmConstraints(decimal minMemGiB, decimal minStorageGiB, int cores = 1)
        {
            this.MinMemGiB = minMemGiB;
            this.MinStorageGiB = minStorageGiB;
            this.Cores = cores;
        }


        public override string ToString()
        {
            var rules = $"({Properties.INF_MEM_GIB}>={this.MinMemGiB})" +
                        $"({Properties.INF_STORAGE_GIB}>={this.MinStorageGiB})" +
                        $"({Properties.INF_CPU_CORES}>={this.Cores})";

            return $"(&{rules})";

        }
    }
}
