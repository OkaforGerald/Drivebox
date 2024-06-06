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
using SharedAPI.RequestFeatures;

namespace Dropbox.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoldersController : ControllerBase
    {
        private readonly IServiceManager serviceManager;

        public FoldersController(IServiceManager serviceManager)
        {
            this.serviceManager = serviceManager;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateFolder([FromBody] CreateFolderDto folder)
        {
            if (folder is null)
            {
                return BadRequest(new ResponseDto<string>
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Errors = new List<string>
                    {
                        "Folder creation object can not be null!"
                    }
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var username = HttpContext?.User?.Identity?.Name;

            await serviceManager.FolderService.CreateFolderAsync(username, folder);

            return Ok(new ResponseDto<string>
            {
                IsSuccessful = true,
                StatusCode = StatusCodes.Status200OK,
                Data = "Folder created successfully!"
            });
        }

        [HttpDelete("{Id:Guid}")]
        [Authorize]
        public async Task<IActionResult> DeleteFolder(Guid Id)
        {
            var username = HttpContext?.User?.Identity?.Name;

            await serviceManager.FolderService.DeleteFolderAsync(username, Id);

            return Ok(new ResponseDto<string>
            {
                IsSuccessful = true,
                StatusCode = StatusCodes.Status200OK,
                Data = "Folder deleted successfully!"
            });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetFolders([FromQuery] RequestParameters parameters)
        {
            var username = HttpContext?.User?.Identity?.Name;
            var response = await serviceManager.FolderService.GetFoldersForUserAsync(username, parameters);
            return Ok(new ResponseDto<List<FolderDto>>
            {
                IsSuccessful = true,
                StatusCode = StatusCodes.Status200OK,
                Data = response
            });
        }

        [HttpGet("{Id:Guid}")]
        [Authorize]
        public async Task<IActionResult> GetFolder(Guid Id)
        {
            var username = HttpContext?.User?.Identity?.Name;
            var response = await serviceManager.FolderService.GetFolderAsync(username, Id);
            return Ok(new ResponseDto<FolderV2Dto>
            {
                IsSuccessful = true,
                StatusCode = StatusCodes.Status200OK,
                Data = response
            });
        }

        [HttpPut("{Id:Guid}")]
        [Authorize]
        public async Task<IActionResult> UpdateFolder(Guid Id, [FromBody] CreateFolderDto dto)
        {
            var username = HttpContext?.User?.Identity?.Name;
            
            await serviceManager.FolderService.UpdateFolderAsync(username, Id, dto);

            return Ok(new ResponseDto<string>
            {
                IsSuccessful = true,
                StatusCode = StatusCodes.Status200OK,
                Data = "Folder updated successfully!"
            });
        }

        [HttpPost("sync")]
        public async Task<IActionResult> GetWhatever(string path)
        {
            var username = HttpContext?.User?.Identity?.Name;

            if (String.IsNullOrEmpty(path))
            {
                return BadRequest();
            }

            await serviceManager.ContentService.SyncLocalFolder(username, path);

            return Ok();
        }
    }
}
