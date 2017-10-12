namespace TicketDesk.Core.Helpers
{
    using System;
    using System.ComponentModel;
    using System.Reflection;

    public static class Enumerations
    {
        public static string GetDescription(Enum enumType)
        {
            string desciption = string.Empty;

            if (enumType != null)
            {
                FieldInfo fi = enumType.GetType().GetField(enumType.ToString());
                DescriptionAttribute[] attributes =
                    (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                desciption = attributes.Length > 0 ? attributes[0].Description : enumType.ToString();
            }

            return desciption;
        }
    }
}
