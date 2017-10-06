
namespace TicketDesk.Core.Enums
{
    using System.ComponentModel;

    public enum TicketCategoryType
    {
        [Description("Facility")]
        Facility = 1,

        [Description("Hardware")]
        Hardware = 2,

        [Description("Software")]
        Software = 3,

        [Description("Network")]
        Network = 4,

        [Description("Customer Support")]
        CustomerSupport = 5,

        [Description("Incident")]
        Incident = 6,

        [Description("Feedback")]
        Feedback = 7
    }

}
