namespace TicketDesk.Domain
{
    using System.Collections.Generic;

    public static class StringExtensions
    {
        public static string ConvertPascalCaseToFriendlyString(this string stringToConvert)
        {
            var friendlyName = new List<char>();
            var name = stringToConvert.ToCharArray();
            for (var index = 0; index < name.Length; index++)
            {
                var c = name[index];
                if (index > 0 && char.IsUpper(c))
                {
                    friendlyName.Add(' ');
                }

                friendlyName.Add(c);
            }

            return new string(friendlyName.ToArray());
        }
    }
}