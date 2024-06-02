using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contracts
{
    public interface IServiceManager
    {
        IAuthService AuthService { get; }

        IFolderService FolderService { get; }

        IContentService ContentService { get; }

        IRequestService RequestService { get; }
    }
}
