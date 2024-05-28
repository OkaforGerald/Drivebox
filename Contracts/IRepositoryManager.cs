using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IRepositoryManager
    {
        IFolderRepository folder {  get; }

        IContentRepository content { get; }

        IUserFolderRepository userFolder { get; }

        Task SaveAsync();
    }
}
