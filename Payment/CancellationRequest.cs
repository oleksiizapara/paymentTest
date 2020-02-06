using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Payment
{
    public class CancellationRequest
    {
        public string Id { get; set; }

        public RequestStatus Status { get; set; }

        public List<PaymentApproval> cancellationApprovals { get; set; }

        public CancellationRequestData Data { get; set; }

        public string RequestedByUserId { get; set; }

        public string RequestedByUserName { get; set; }

        public DateTimeOffset RequestedAt { get; set; }
    }
}
