using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace YagnaSharpApi.Utils
{
    public struct VmConstraints
    {
        public decimal MinMemGiB { get; set; }
        public decimal MinStorageGiB { get; set; }
        public int Cores { get; set; }
        public string RuntimeName { get; set; }

        public VmConstraints(decimal minMemGiB, decimal minStorageGiB, int cores = 1, string runtimeName = PropertyValues.RUNTIME_NAME_VM)
        {
            this.MinMemGiB = minMemGiB;
            this.MinStorageGiB = minStorageGiB;
            this.Cores = cores;
            this.RuntimeName = runtimeName;
        }


        public override string ToString()
        {
            var rules = $"({Properties.INF_MEM_GIB}>={this.MinMemGiB.ToString(CultureInfo.InvariantCulture)})" +
                        $"({Properties.INF_STORAGE_GIB}>={this.MinStorageGiB.ToString(CultureInfo.InvariantCulture)})" +
                        $"({Properties.INF_CPU_CORES}>={this.Cores.ToString(CultureInfo.InvariantCulture)})" + 
                        $"({Properties.RUNTIME_NAME}={this.RuntimeName})";

            return $"(&{rules})";

        }
    }
}
