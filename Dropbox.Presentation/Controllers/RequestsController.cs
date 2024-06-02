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
using SharedAPI.DataTransfer;

namespace Dropbox.Presentation.Controllers
{
    [ApiController]
    [Route("api/folders/{FolderId}/[controller]")]
    public class RequestsController : ControllerBase
    {
        private readonly IServiceManager serviceManager;
        public RequestsController(IServiceManager serviceManager)
        {
            this.serviceManager = serviceManager;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetRequests(Guid FolderId)
        {
            var username = HttpContext.User?.Identity?.Name;

            var response = await serviceManager.RequestService.GetRequestsForFolder(username, FolderId);

            return Ok(new ResponseDto<List<RequestDto>>
            {
                IsSuccessful = true,
                StatusCode = StatusCodes.Status200OK,
                Data = response
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RequestAccess(Guid FolderId)
        {
            var username = HttpContext.User?.Identity?.Name;

            await serviceManager.RequestService.RequestAccess(username, FolderId);

            return Ok(new ResponseDto<string>
            {
                IsSuccessful = true,
                StatusCode = StatusCodes.Status200OK,
                Data = "Request made successfully!"
            });
        }

        [HttpPut("acknowledge")]
        [Authorize]
        public async Task<IActionResult> AckRequest(Guid FolderId, Guid RequestId)
        {
            var username = HttpContext.User?.Identity?.Name;

            await serviceManager.RequestService.AcknowledgeRequest(username, FolderId, RequestId);

            return Ok(new ResponseDto<string>
            {
                IsSuccessful = true,
                StatusCode = StatusCodes.Status200OK,
                Data = "Request accepted successfully!"
            });
        }

        [HttpPut("decline")]
        [Authorize]
        public async Task<IActionResult> DeclineRequest(Guid FolderId, Guid RequestId)
        {
            var username = HttpContext.User?.Identity?.Name;

            await serviceManager.RequestService.DeclineRequest(username, FolderId, RequestId);

            return Ok(new ResponseDto<string>
            {
                IsSuccessful = true,
                StatusCode = StatusCodes.Status200OK,
                Data = "Request declined successfully!"
            });
        }
    }
}
