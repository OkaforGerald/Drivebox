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
    public class UserFolderRepository : RepositoryBase<UserFolders>, IUserFolderRepository
    {
        public UserFolderRepository(RepositoryContext context) : base(context)
        {
        }

        public void CreateUserFolder(UserFolders folder)
        {
            Create(folder);
        }

        public void DeleteUserFolder(UserFolders folder)
        {
            Delete(folder);
        }

        public async Task<UserFolders> GetUserPermissionsForFolder(string userId, Guid FolderId, bool trackChanges)
        {
            return await FindByCondition(x => x.UserId.Equals(userId) &&  x.FolderId.Equals(FolderId), trackChanges)
                .Include(x => x.User)
                .Include(x => x.Folder)
                .FirstOrDefaultAsync();
        }

        public async Task<List<UserFolders>> GetCollaboratorsForFolder(Guid FolderId,  bool trackChanges)
        {
            return await FindByCondition(x => x.FolderId.Equals(FolderId), trackChanges)
                .Include(x => x.User)
                .ToListAsync();
        }
    }
}
