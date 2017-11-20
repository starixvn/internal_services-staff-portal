namespace TicketDesk.Domain.Infrastructure
{
    using System;

    /// <summary>
    /// Marker attribute indicating if comments are required for TicketActivityEnum
    /// </summary>
    [AttributeUsageAttribute(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class CommentRequiredAttribute : Attribute
    {
    }
}