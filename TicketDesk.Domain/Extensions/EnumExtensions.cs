namespace TicketDesk.Domain.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;

    public static class EnumExtensions
    {
        /// <summary>
        /// Get description for enum
        /// Wrote by Raymond.Le
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets the display name for an enum.
        /// </summary>
        /// <param name="enumeration">The enumeration.</param>
        /// <returns>The text of the DisplayAttribute if applicable, otherwise returns the value as a string.</returns>
        /// <remarks>If used on a sepcifc value from the enumeration, will return the display name for that value.
        /// If used on an enum as a whole, will return the display name of the enum itself.</remarks>
        public static string GetDisplayName(this Enum enumeration)
        {
            var type = enumeration.GetType();
            var memberInfo = type.GetMember(enumeration.ToString());

            // if there is member information
            if (memberInfo.Length > 0)
            {
                var attributes = memberInfo[0].GetCustomAttributes(typeof(DisplayAttribute), false);

                if (attributes.Length > 0)
                {
                    return ((DisplayAttribute)attributes.First()).GetName();
                }
            }

            return enumeration.ToString();
        }

        /// <summary>
        /// Gets the enumerator from the display name passed in
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="displayName">The display name.</param>
        /// <returns>T.</returns>
        public static T GetEnumFromDisplayName<T>(this string displayName)
        {
            // get the member info of the enum
            var memberInfos = typeof(T).GetMembers();

            if (memberInfos.Length > 0)
            {
                // loop through the member info classes
                foreach (var memberInfo in memberInfos)
                {
                    // get the custom attributes of the member info
                    object[] attributes = memberInfo.GetCustomAttributes(typeof(DisplayAttribute), false);

                    // if there are attributes
                    // if the display attribute is equal to the display name, return the enum
                    if (attributes.Length > 0 && ((DisplayAttribute)attributes[0]).GetName() == displayName)
                    {
                        return (T)Enum.Parse(typeof(T), memberInfo.Name);
                    }
                }
            }

            // this means the enum was not found from the display name, so return the default
            return default(T);
        }

        public static IEnumerable<Enum> GetFlags(this Enum input)
        {
            return Enum.GetValues(input.GetType()).Cast<Enum>().Where(input.HasFlag);
        }
    }
}