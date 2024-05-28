using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Services.Contracts
{
    public interface IContentService
    {
        Task CreateContent(string username, Guid FolderId, IFormFile file);
    }
}
