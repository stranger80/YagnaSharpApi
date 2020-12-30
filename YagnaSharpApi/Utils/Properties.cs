﻿using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Utils
{
    public class Properties
    {
        public const string NODE_ID_NAME = "golem.node.id.name";
        public const string NODE_DEBUG_SUBNET = "golem.node.debug.subnet";

        public const string INF_MEM_GIB = "golem.inf.mem.gib";
        public const string INF_STORAGE_GIB = "golem.inf.storage.gib";
        public const string INF_CPU_CORES = "golem.inf.cpu.cores";

        public const string COM_SCHEME = "golem.com.scheme";
        public const string COM_PRICING_MODEL = "golem.com.pricing.model";
        public const string COM_PRICING_MODEL_LINEAR_COEFFS = "golem.com.pricing.model.linear.coeffs";
        public const string COM_USAGE_VECTOR = "golem.com.usage.vector";


        public const string ACTIVITY_CAPS_TRANSFER_PROTOCOL = "golem.activity.caps.transfer.protocol";

        public const string SRV_COMP_EXPIRATION = "golem.srv.comp.expiration";
        public const string SRV_COMP_TASK_PACKAGE = "golem.srv.comp.task_package";
        public const string SRV_COMP_VM_PACKAGE_FORMAT = "golem.srv.comp.vm.package_format";


     
    }

    public class Counters
    {
        public const string DURATION_SEC = "golem.usage.duration_sec";
        public const string CPU_SEC = "golem.usage.cpu_sec";
        public const string STORAGE_GIB = "golem.usage.storage_gib";

    }
}
