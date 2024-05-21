using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;

namespace Repository
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly RepositoryContext repositoryContext;
        private readonly Lazy<IFolderRepository> Folder;

        public RepositoryManager(RepositoryContext repositoryContext)
        {
            this.repositoryContext = repositoryContext;
            Folder = new Lazy<IFolderRepository>(new FolderRepository(repositoryContext));
        }

        public IFolderRepository folder => Folder.Value;

        public async Task SaveAsync()
        {
            await repositoryContext.SaveChangesAsync();
        }
    }
}
