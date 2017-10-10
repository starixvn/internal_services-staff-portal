namespace TicketDesk.Core.Enumerations
{
    using System.ComponentModel;

    public enum CustomFieldTypes
    {
        [Description("String")]
        String = 1,

        [Description("Int")]
        Int = 2,

        [Description("Bool")]
        Bool = 3,

        [Description("List")]
        List = 4,

        [Description("Date/Time")]
        DateTime = 5,

        [Description("Text area")]
        TextArea = 6
    }
}