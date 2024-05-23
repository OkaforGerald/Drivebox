using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Extensions;
using SharedAPI.RequestFeatures;

namespace Repository
{
    public class FolderRepository : RepositoryBase<Folder>, IFolderRepository
    {
        public FolderRepository(RepositoryContext context) : base(context)
        {
        }

        public void CreateFolder(Folder Folder)
        {
            Create(Folder);
        }

        public void DeleteFolder(Folder Folder)
        {
            Delete(Folder);
        }

        public async Task<Folder> GetFolder(Guid FolderId, bool trackChanges)
        {
            return await FindByCondition(x => x.Id == FolderId, trackChanges)
                .Include(x => x.Owner)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Folder>> GetFoldersByUser(string UserId, RequestParameters parameters, bool trackChanges)
        {
            return await FindByCondition(x => x.OwnerId.Equals(UserId), trackChanges)
                .Include(x => x.Owner)
                .Filter(parameters.FolderType)
                .Search(parameters.SearchTerm)
                .Sort<Folder>(parameters.OrderBy)
                .ToListAsync();
        }

        public async Task<List<Folder>> GetFoldersByUser(string UserId, bool trackChanges)
        {
            return await FindByCondition(x => x.OwnerId.Equals(UserId), trackChanges)
                .ToListAsync();
        }
    }
}
