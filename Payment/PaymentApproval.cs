using System;

namespace Payment
{
    public class PaymentApproval
    {
        public string Id { get; set; }

        public ApprovalStatus Status { get; set; }

        public string Reason { get; set; }

        public string UserId { get; set; }

        public string UserName { get; set; }

        public DateTimeOffset ApprovedAt { get; set; }
    }
}
