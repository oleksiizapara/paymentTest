using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Payment
{
    public class ModificationRequest
    {
        public string Id { get; private set; }

        public RequestStatus Status { get; set; }

        public List<PaymentApproval> Approvals { get; set; }

        public ModificationRequestData Data { get; set; }

        public string RequestedByUserId { get; set; }

        public string RequestedByUserName { get; set; }

        public DateTimeOffset RequestedAt { get; set; }
    }
}
