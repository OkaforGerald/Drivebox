using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Contracts;

namespace Repository
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly RepositoryContext repositoryContext;
        private readonly Lazy<IFolderRepository> Folder;
        private readonly Lazy<IContentRepository> Content;
        private readonly Lazy<IUserFolderRepository> UserFolder;

        public RepositoryManager(RepositoryContext repositoryContext)
        {
            this.repositoryContext = repositoryContext;
            Folder = new Lazy<IFolderRepository>(new FolderRepository(repositoryContext));
            UserFolder = new Lazy<IUserFolderRepository>(new UserFolderRepository(repositoryContext));
            Content = new Lazy<IContentRepository>(new ContentRepository(repositoryContext));
        }

        public IFolderRepository folder => Folder.Value;

        public IUserFolderRepository userFolder => UserFolder.Value;

        public IContentRepository content => Content.Value;

        public async Task SaveAsync()
        {
            await repositoryContext.SaveChangesAsync();
        }
    }
}
