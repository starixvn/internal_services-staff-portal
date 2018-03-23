namespace TicketDesk.Core.Enumerations
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using Resources;

    public enum TicketCategoryTypes
    {
        [Description("Facility")]
        [Display(Name = nameof(Messages.Facility), ResourceType = typeof(Messages))]
        Facility = 1,

        [Description("Hardware")]
        [Display(Name = nameof(Messages.Hardware), ResourceType = typeof(Messages))]
        Hardware = 2,

        [Description("Software")]
        [Display(Name = nameof(Messages.Software), ResourceType = typeof(Messages))]
        Software = 3,

        [Description("Network")]
        [Display(Name = nameof(Messages.Network), ResourceType = typeof(Messages))]
        Network = 4,

        [Description("Customer Support")]
        [Display(Name = nameof(Messages.CustomerSupport), ResourceType = typeof(Messages))]
        CustomerSupport = 5,

        [Description("Incident")]
        [Display(Name = nameof(Messages.Incident), ResourceType = typeof(Messages))]
        Incident = 6,

        [Description("Feedback")]
        [Display(Name = nameof(ViewLang.Feedback), ResourceType = typeof(ViewLang))]
        Feedback = 7
    }
}