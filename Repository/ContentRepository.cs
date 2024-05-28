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
    public class ContentRepository : RepositoryBase<Content>, IContentRepository
    {
        public ContentRepository(RepositoryContext context) : base(context)
        {}

        public void CreateContent(Content content)
        {
            Create(content);
        }

        public void DeleteContent(Content content)
        {
            Delete(content);
        }

        public async Task<List<Content>> GetContentsByFolderAsync(Guid FolderId, bool trackChanges)
        {
            return await FindByCondition(x => x.FolderId.Equals(FolderId), trackChanges)
                .ToListAsync();
        }

        public async Task<Content> GetContentAsync(Guid FolderId, Guid ContentId,  bool trackChanges)
        {
            return await FindByCondition(x => x.FolderId.Equals(FolderId) && x.Id.Equals(ContentId), trackChanges)
                .FirstOrDefaultAsync();
        }
    }
}
