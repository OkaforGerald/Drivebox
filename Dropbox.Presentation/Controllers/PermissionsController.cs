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
    [Route("api/folders/{Id:Guid}/[controller]")]
    public class PermissionsController : ControllerBase
    {
        private readonly IServiceManager serviceManager;

        public PermissionsController(IServiceManager serviceManager)
        {
            this.serviceManager = serviceManager;
        }

        [HttpPost("{username}/grant-write")]
        [Authorize]
        public async Task<IActionResult> GrantWriteAccess(Guid Id, string username)
        {
            var user = HttpContext?.User?.Identity?.Name;

            await serviceManager.RequestService.GrantWriteAccess(user, username, Id);

            return Ok(new ResponseDto<string>
            {
                IsSuccessful = true,
                StatusCode = StatusCodes.Status200OK,
                Data = "Write access granted!"
            });
        }

        [HttpPost("{username}/revoke")]
        [Authorize]
        public async Task<IActionResult> RevokeAccess(Guid Id, string username)
        {
            var user = HttpContext?.User?.Identity?.Name;

            await serviceManager.RequestService.RevokeAccess(user, username, Id);

            return Ok(new ResponseDto<string>
            {
                IsSuccessful = true,
                StatusCode = StatusCodes.Status200OK,
                Data = "Access revoked!"
            });
        }
    }
}
