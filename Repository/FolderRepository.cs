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
                .FirstOrDefaultAsync();
        }

        public async Task<List<Folder>> GetFoldersByUser(string UserId, bool trackChanges)
        {
            return await FindByCondition(x => x.OwnerId.Equals(UserId), trackChanges)
                .ToListAsync();
        }
    }
}
