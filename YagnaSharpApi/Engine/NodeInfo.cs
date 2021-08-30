using System;
using System.Collections.Generic;
using System.Text;
using YagnaSharpApi.Entities;
using YagnaSharpApi.Utils;

namespace YagnaSharpApi.Engine
{
    public class NodeInfo
    {
        /// <summary>
        /// Node name
        /// </summary>
        public string Name { get; set; }

        public NodeInfo(AgreementEntity agreement)
        {
            if (agreement?.Offer?.Properties?.ContainsKey(Properties.NODE_ID_NAME) ?? false)
                this.Name = agreement.Offer.Properties[Properties.NODE_ID_NAME].ToString();
        }

    }
}
