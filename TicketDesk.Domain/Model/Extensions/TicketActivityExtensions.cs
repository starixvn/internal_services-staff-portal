namespace TicketDesk.Domain.Model
{
    using Infrastructure;
    using System.Linq;

    public static class TicketActivityExtensions
    {
        public static bool IsCommentRequired(this TicketActivity enumeration)
        {
            var req = false;
            var type = enumeration.GetType();
            var memberInfo = type.GetMember(enumeration.ToString());

            // if there is member information
            if (memberInfo.Length > 0)
            {
                req = memberInfo[0].GetCustomAttributes(typeof(CommentRequiredAttribute), false).Any();
            }

            return req;
        }
    }
}