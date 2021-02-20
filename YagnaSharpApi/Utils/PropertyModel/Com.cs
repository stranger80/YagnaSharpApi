using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YagnaSharpApi.Utils.PropertyModel
{
    public class Com
    {

        public class ComLinear
        {
            public const string FIXED = "_fixed";

            public string Scheme { get; } = PropertyValues.COM_PRICING_MODEL_LINEAR;

            /// <summary>
            /// price coefficients indexed by usage counter name.
            /// NOTE: the fixed element is indexed by '_fixed'
            /// </summary>
            public IDictionary<string, decimal> Coeffs { get; set; }

            public static ComLinear FromProperties(IDictionary<string, object> props, Com com)
            {
                var result = new ComLinear();

                if(props.ContainsKey(Properties.COM_PRICING_MODEL_LINEAR_COEFFS))
                {
                    result.Coeffs = new Dictionary<string, decimal>();

                    var coeffs = (object[])props[Properties.COM_PRICING_MODEL_LINEAR_COEFFS];

                    result.Coeffs.Add(FIXED, (decimal)Double.Parse(coeffs[0].ToString()));

                    for(int i=0; i<com.UsageVector.Length; i++)
                    {
                        result.Coeffs.Add(com.UsageVector[i], (decimal)Double.Parse(coeffs[i+1].ToString()));
                    }
                }

                return result;
            }
        }


        public static Com FromProperties(IDictionary<string, object> props)
        {
            var result = new Com();

            if(props.ContainsKey(Properties.COM_USAGE_VECTOR))
            {
                result.UsageVector = ((object[])props[Properties.COM_USAGE_VECTOR]).Select(o => o.ToString()).ToArray();
            }

            if (props.ContainsKey(Properties.COM_SCHEME))
            {
                result.Scheme = props[Properties.COM_SCHEME].ToString();
            }

            if (props.ContainsKey(Properties.COM_PRICING_MODEL))
            {
                switch(props[Properties.COM_PRICING_MODEL].ToString())
                {
                    case PropertyValues.COM_PRICING_MODEL_LINEAR:
                        result.Linear = ComLinear.FromProperties(props, result);
                        break;
                    default:
                        throw new Exception($"Unknown {Properties.COM_PRICING_MODEL} value: {props[Properties.COM_PRICING_MODEL].ToString()}");
                }
            }

            return result;
        }

        public string[] UsageVector { get; set; }
        public string Scheme { get; set; }
        public ComLinear Linear { get; set; }

    }
}
