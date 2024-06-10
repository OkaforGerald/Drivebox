using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;

namespace Contracts
{
    public interface IBackupRepository
    {
        void CreateBackup(Backup backup);

        void DeleteBackup(Backup backup);

        Task<Backup> GetBackup(Guid FolderId, bool trackChanges);
    }
}
