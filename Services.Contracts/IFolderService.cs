using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedAPI.DataTransfer;
using SharedAPI.RequestFeatures;

namespace Services.Contracts
{
    public interface IFolderService
    {
        Task CreateFolderAsync(string username, CreateFolderDto folder);

        Task DeleteFolderAsync(string username, Guid FolderId);

        Task<List<FolderDto>> GetFoldersForUsersAsync(string username, RequestParameters parameters);

        Task<FolderDto> GetFolderAsync(string username, Guid Id);
    }
}
