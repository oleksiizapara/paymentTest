using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Payment
{
    public class Payment
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public string Uetr { get; set; }

        public CancellationRequest CancellationRequest { get; set; }

        public ModificationRequest ModificationRequest { get; set; }

        public List<PaymentLogRow> Logs { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
