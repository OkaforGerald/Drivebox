using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Exceptions
{
    public class FolderNotFoundException : NotFoundException
    {
        public FolderNotFoundException(Guid Id) : base($"Folder with Id {Id} could not be found!")
        {
        }
    }
}
