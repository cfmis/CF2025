using System;
using System.Linq;
using System.ServiceModel;
using System.Collections.Generic;
using System.Xml;
using System.ServiceModel.Description;

namespace CF.Framework.Utility
{
    /// <summary>
    /// Wcf堆翑濬
    /// </summary>
    public class WcfServiceProxy
    {
        /// <summary>
        /// 雄怓斐膘Wcf諦誧傷測燴妗瞰
        /// </summary>
        /// <typeparam name="T">Contract/諉諳</typeparam>
        /// <param name="uri">Wcf督昢華硊</param>
        /// <returns>測燴妗瞰</returns>
        public static T CreateServiceProxy<T>(string uri)
        {
            var key = string.Format("{0} - {1}", typeof(T), uri);

            if (Caching.Get(key) == null)
            {
                var binding = new BasicHttpBinding();
                binding.MaxReceivedMessageSize = maxReceivedMessageSize;
                binding.ReaderQuotas = new XmlDictionaryReaderQuotas();
                binding.ReaderQuotas.MaxStringContentLength = maxReceivedMessageSize;
                binding.ReaderQuotas.MaxArrayLength = maxReceivedMessageSize;
                binding.ReaderQuotas.MaxBytesPerRead = maxReceivedMessageSize;
                binding.OpenTimeout = timeout;
                binding.ReceiveTimeout = timeout;
                binding.SendTimeout = timeout;

                var chan = new ChannelFactory<T>(binding, new EndpointAddress(uri));

                foreach (OperationDescription op in chan.Endpoint.Contract.Operations)
                {
                    var dataContractBehavior = op.Behaviors.Find<DataContractSerializerOperationBehavior>();
                    if (dataContractBehavior != null)
                        dataContractBehavior.MaxItemsInObjectGraph = int.MaxValue;
                }


                chan.Open();

                var service = chan.CreateChannel();
                Caching.Set(key, service);

                return service;
            }
            else
            {
                return (T)Caching.Get(key);
            }
        }

        private const int maxReceivedMessageSize = 2147483647;
        private static TimeSpan timeout = TimeSpan.FromMinutes(10);
    }
}
