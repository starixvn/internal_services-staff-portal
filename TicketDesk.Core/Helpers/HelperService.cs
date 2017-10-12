namespace TicketDesk.Core.Helpers
{
    using System;
    using System.Collections.Generic;

    public static class HelperService
    {
        public static IList<string> ConvertValuesToList(string values)
        {
            return values.Split(new string[] { "\r\n" }, StringSplitOptions.None);
        }
    }
}