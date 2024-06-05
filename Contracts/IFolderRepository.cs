using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;
using SharedAPI.RequestFeatures;

namespace Contracts
{
    public interface IFolderRepository
    {
        void CreateFolder(Folder Folder);

        void DeleteFolder(Folder Folder);

        Task<Folder> GetFolder(Guid FolderId, bool trackChanges);
        
        Task<List<Folder>> GetFoldersByUser(string UserId, RequestParameters parameters, bool trackChanges);

        Task<Folder> GetBaseFolder(Guid Id, bool trackChanges);

        Task<List<Folder>> GetChildFolders(Guid Id, string OwnerId, bool trackChanges);

        Task<List<Folder>> GetFoldersByUser(string UserId, bool trackChanges);
    }
}
