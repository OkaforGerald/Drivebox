using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class RequestRepository : RepositoryBase<Request>, IRequestRepository
    {
        public RequestRepository(RepositoryContext context) : base(context)
        {
        }

        public void CreateRequest(Request request)
        {
            Create(request);
        }

        public void DeleteRequest(Request request)
        {
            Delete(request);
        }

        public async Task<List<Request>> GetRequestsForFolderAsync(Guid FolderId, bool trackChanges)
        {
            return await FindByCondition(x => x.FolderId.Equals(FolderId), trackChanges)
                .OrderByDescending(x => x.CreatedAt)
                .Include(x => x.Requester)
                .ToListAsync();
        }

        public async Task<Request> GetRequestAsync(Guid FolderId, Guid RequestId, bool trackChanges)
        {
            return await FindByCondition(x => x.FolderId.Equals(FolderId) && x.Id.Equals(RequestId), trackChanges)
                .Include(x => x.Requester)
                .FirstOrDefaultAsync();
        }
    }
}
