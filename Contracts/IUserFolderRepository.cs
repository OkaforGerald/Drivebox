using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;

namespace Contracts
{
    public interface IUserFolderRepository
    {
        void CreateUserFolder(UserFolders folder);

        void DeleteUserFolder(UserFolders folder);

        Task<UserFolders> GetUserPermissionsForFolder(string userId, Guid FolderId, bool trackChanges);

        Task<List<UserFolders>> GetCollaboratorsForFolder(Guid FolderId, bool trackChanges);
    }
}
