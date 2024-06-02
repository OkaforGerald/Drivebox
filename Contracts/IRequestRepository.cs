using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;

namespace Contracts
{
    public interface IRequestRepository
    {
        void CreateRequest(Request request);

        void DeleteRequest(Request request);

        Task<List<Request>> GetRequestsForFolderAsync(Guid FolderId, bool trackChanges);

        Task<Request> GetRequestAsync(Guid FolderId, Guid RequestId, bool trackChanges);
    }
}
