using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Tests
{
    public static class TestConstants
    {
        public const string SUBNET_TAG = "devnet-beta";
        public const string VM_TASK_DEFAULT_PAYLOAD_HASH = "9a3b5d67b0b27746283cb5f287c13eab1beaa12d92a9f536b747c7ae"; //"d646d7b93083d817846c2ae5c62c72ca0507782385a2e29291a3d376";
        public const string VM_TASK_PACKAGE = "hash://sha3:9a3b5d67b0b27746283cb5f287c13eab1beaa12d92a9f536b747c7ae:http://yacn2.dev.golem.network:8000/local-image-c76719083b.gvmi";
        public const string PAYMENT_PLATFORM = /*"erc20-rinkeby-tglm"; //*/ "zksync-rinkeby-tglm"; // is failing, erc20 is the fallback
    }
}
