using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;
using SharedAPI.DataTransfer;
using SharedAPI.RequestFeatures;

namespace Services.Contracts
{
    public interface IFolderService
    {
        Task CreateFolderAsync(string username, CreateFolderDto folder);

        Task DeleteFolderAsync(string username, Guid FolderId);

        Task<List<FolderDto>> GetFoldersForUserAsync(string username, RequestParameters parameters);

        Task<FolderV2Dto> GetFolderAsync(string username, Guid Id);

        Task UpdateFolderAsync(string username, Guid Id, CreateFolderDto update);
    }
}
