using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;

namespace Contracts
{
    public interface IContentRepository
    {
        void CreateContent(Content content);

        void DeleteContent(Content content);

        Task<List<Content>> GetContentsByFolderAsync(Guid FolderId, bool trackChanges);

        Task<Content> GetContentAsync(Guid FolderId, Guid ContentId, bool trackChanges);
    }
}
