using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Exceptions
{
    public class UnauthorizedFolderException : Exception
    {
        public UnauthorizedFolderException(Guid Id) : base($"You're not authorized to access Folder with Id {Id}")
        {
            
        }
    }
}
