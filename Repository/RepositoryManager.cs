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
        private readonly Lazy<IRequestRepository> Request;
        private readonly Lazy<IUserFolderRepository> UserFolder;
        private readonly Lazy<IBackupRepository> Backup;

        public RepositoryManager(RepositoryContext repositoryContext)
        {
            this.repositoryContext = repositoryContext;
            Folder = new Lazy<IFolderRepository>(new FolderRepository(repositoryContext));
            UserFolder = new Lazy<IUserFolderRepository>(new UserFolderRepository(repositoryContext));
            Content = new Lazy<IContentRepository>(new ContentRepository(repositoryContext));
            Request = new Lazy<IRequestRepository>(new RequestRepository(repositoryContext));
            Backup = new Lazy<IBackupRepository>(new BackupRepository(repositoryContext));
        }

        public IFolderRepository folder => Folder.Value;

        public IUserFolderRepository userFolder => UserFolder.Value;

        public IContentRepository content => Content.Value;

        public IRequestRepository request => Request.Value;

        public IBackupRepository backup => Backup.Value;

        public async Task SaveAsync()
        {
            await repositoryContext.SaveChangesAsync();
        }
    }
}
