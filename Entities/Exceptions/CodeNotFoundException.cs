using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Exceptions
{
    public class CodeNotFoundException : NotFoundException
    {
        public CodeNotFoundException(Guid Id) : base($"Content with Id {Id} can not be found!")
        {
        }
    }
}
