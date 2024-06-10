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
    public class BackupRepository : RepositoryBase<Backup>, IBackupRepository
    {
        public BackupRepository(RepositoryContext context) : base(context)
        {
        }

        public void CreateBackup(Backup backup)
        {
            Create(backup);
        }

        public void DeleteBackup(Backup backup)
        {
            Delete(backup);
        }

        public async Task<Backup> GetBackup(Guid FolderId, bool trackChanges)
        {
            return await FindByCondition(x => x.FolderId == FolderId, trackChanges)
                .FirstOrDefaultAsync();
        }
    }
}
