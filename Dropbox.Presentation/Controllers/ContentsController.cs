using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using SharedAPI;

namespace Dropbox.Presentation.Controllers
{
    [ApiController]
    [Route("api/folders/{FolderId:Guid}/[controller]")]
    public class ContentsController : ControllerBase
    {
        private readonly IServiceManager serviceManager;

        public ContentsController(IServiceManager serviceManager)
        {
            this.serviceManager = serviceManager;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateContent(Guid FolderId, IFormFile file)
        {
            var username = HttpContext?.User?.Identity?.Name;
            if (file is null || file.Length == 0)
            {
                return BadRequest(new ResponseDto<string>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Errors = new List<string> { "File can not be empty" }
                });
            }

            await serviceManager.ContentService.CreateContent(username, FolderId, file);

            return Ok(new ResponseDto<string>
            {
                IsSuccessful = true,
                StatusCode = StatusCodes.Status200OK,
                Data = "File Created Successfully!"
            });
        }

        [HttpDelete("{ContentId}")]
        [Authorize]
        public async Task<IActionResult> DeleteContentAsync(Guid FolderId, Guid ContentId)
        {
            var username = HttpContext?.User?.Identity?.Name;

            await serviceManager.ContentService.DeleteContentAsync(username, FolderId, ContentId);

            return Ok(new ResponseDto<string>
            {
                IsSuccessful =true,
                StatusCode = StatusCodes.Status200OK,
                Data = "File Deleted Successfully!"
            });
        }
    }
}
