namespace TicketDesk.Service.Interface
{
    using System;
    using Domain.Model;

    public interface IIdUploaderService
    {
        bool CreateCustomer(UploadedUserId uploadedUserId);

        void MoveFiles(Guid tempId, string pendingFolder, string idUploadedFolder);
    }
}
