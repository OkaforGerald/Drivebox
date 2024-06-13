using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedAPI.DataTransfer;

namespace Services.Contracts
{
    public interface IRequestService
    {
        Task RequestAccess(string username, Guid FolderId);

        Task<List<RequestDto>> GetRequestsForFolder(string username, Guid FolderId);

        Task AcknowledgeRequest(string username, Guid FolderId, Guid RequestId);

        Task DeclineRequest(string username, Guid FolderId, Guid RequestId);

        Task GrantWriteAccess(string granter, string requester, Guid FolderId);

        Task RevokeAccess(string revoker, string revoked, Guid FolderId);
    }
}
