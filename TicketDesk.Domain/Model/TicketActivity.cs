namespace TicketDesk.Domain.Model
{
    using Infrastructure;
    using System;
    using System.ComponentModel.DataAnnotations;
    using TicketDesk.Localization.Client;
    using TicketDesk.Localization.Domain;

    [Flags]
    public enum TicketActivity
    {
        None = 0,

        [CommentRequired]
        [Display(Name = "TicketActivity_Comment", ResourceType = typeof(Strings))]
        AddComment = 1,

        [CommentRequired]
        [Display(Name = "TicketActivity_ProvideInfo", ResourceType = typeof(Strings))]
        SupplyMoreInfo = 2,

        [Display(Name = "TicketActivity_CancelMoreInfo", ResourceType = typeof(Strings))]
        CancelMoreInfo = 4,

        [CommentRequired]
        [Display(Name = "TicketActivity_RequestMoreInfo", ResourceType = typeof(Strings))]
        RequestMoreInfo = 8,

        [Display(Name = "TicketActivity_TakeOver", ResourceType = typeof(Strings))]
        TakeOver = 16,

        [CommentRequired]
        [Display(Name = "TicketActivity_Resolve", ResourceType = typeof(Strings))]
        Resolve = 32,

        [Display(Name = "TicketActivity_Assign", ResourceType = typeof(Strings))]
        Assign = 64,

        [Display(Name = "TicketActivity_ReAssign", ResourceType = typeof(Strings))]
        ReAssign = 128,

        [Display(Name = "TicketActivity_Pass", ResourceType = typeof(Strings))]
        Pass = 256,

        [Display(Name = "TicketActivity_Close", ResourceType = typeof(Strings))]
        Close = 512,

        [CommentRequired]
        [Display(Name = "TicketActivity_ReOpen", ResourceType = typeof(Strings))]
        ReOpen = 1024,

        [Display(Name = "TicketActivity_GiveUp", ResourceType = typeof(Strings))]
        [CommentRequired]
        GiveUp = 2048,

        [CommentRequired]
        [Display(Name = "TicketActivity_ForceClose", ResourceType = typeof(Strings))]
        ForceClose = 4096,

        [Display(Name = "TicketActivity_EditAttachments", ResourceType = typeof(Strings))]
        ModifyAttachments = 8192,

        [Display(Name = "TicketActivity_Edit", ResourceType = typeof(Strings))]
        EditTicketInfo = 16384,

        [Display(Name = "TicketActivity_Create", ResourceType = typeof(Strings))]
        Create = 32768,

        [Display(Name = "TicketActivity_CreateOnBehalfOf", ResourceType = typeof(Strings))]
        CreateOnBehalfOf = 65536,

        [Display(Name = nameof(Resources.TicketActivity_ChangeStatus), ResourceType = typeof(Resources))]
        ChangeStatus = 131072
    }
}