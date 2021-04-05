using System;
using System.Collections.Generic;
using System.Text;

namespace YagnaSharpApi.Tests
{
    public static class TestConstants
    {
        public const string SUBNET_TAG = "devnet-beta.1";
        public const string VM_TASK_PACKAGE = "hash://sha3:9a3b5d67b0b27746283cb5f287c13eab1beaa12d92a9f536b747c7ae:http://3.249.139.167:8000/local-image-c76719083b.gvmi";
        public const string PAYMENT_PLATFORM = /*"erc20-rinkeby-tglm"; //*/ "zksync-rinkeby-tglm"; // is failing, erc20 is the fallback
    }
}
