using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Payment
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum RequestStatus
    {
        NotStarted,
        Submitted,
        PendingApproval,
        Approved,
        Send
    }
}
