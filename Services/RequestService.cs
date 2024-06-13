using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudinaryDotNet.Actions;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Services.Contracts;
using SharedAPI.DataTransfer;

namespace Services
{
    public class RequestService : IRequestService
    {
        private readonly IRepositoryManager manager;
        private readonly UserManager<User> userManager;

        public RequestService(IRepositoryManager manager, UserManager<User> userManager)
        {
            this.manager = manager;
            this.userManager = userManager;
        }

        public async Task RequestAccess(string username, Guid FolderId)
        {
            var user = await userManager.FindByNameAsync(username);

            var folder = await manager.folder.GetFolder(FolderId, false);

            if(folder is null)
            {
                throw new FolderNotFoundException(FolderId);
            }

            var request = new Request
            {
                CreatedAt = DateTime.Now,
                RequesterId = user.Id,
                FolderId = folder.Id,
                Status = RequestStatus.AwaitingAck
            };

            manager.request.CreateRequest(request);
            await manager.SaveAsync();
        }

        public async Task<List<RequestDto>> GetRequestsForFolder(string username, Guid FolderId)
        {
            var user = await userManager.FindByNameAsync(username);

            var folder = await manager.folder.GetFolder(FolderId, false);

            if(folder is null)
            {
                throw new FolderNotFoundException(FolderId);
            }

            if (!folder.OwnerId.Equals(user.Id))
            {
                throw new UnauthorizedAction("Unauthorized to perform action on folder!");
            }

            var requests = await manager.request.GetRequestsForFolderAsync(FolderId, false);
            var response = requests.Select(x => new RequestDto
            {
                Id = x.Id,
                FolderId = x.FolderId,
                Requester = x.Requester?.UserName,
                Status = x.Status,
                CreatedAt = x.CreatedAt
            }).ToList();

            return response;
        }

        public async Task AcknowledgeRequest(string username, Guid FolderId, Guid RequestId)
        {
            var user = await userManager.FindByNameAsync(username);

            var folder = await manager.folder.GetFolder(FolderId, false);

            if (folder is null)
            {
                throw new FolderNotFoundException(FolderId);
            }

            if (!folder.OwnerId.Equals(user.Id))
            {
                throw new UnauthorizedAction("Unauthorized to perform action on folder!");
            }

            var request = await manager.request.GetRequestAsync(FolderId, RequestId, true);

            if(request is null)
            {
                throw new NotFoundException($"Request with Id {RequestId} could not be found!");
            }

            if(request.Status == RequestStatus.AwaitingAck)
            {
                request.Status = RequestStatus.Accepted;

                var userFolder = new UserFolders
                {
                    UserId = request.RequesterId,
                    FolderId = FolderId,
                    Permissions = Permissions.Read,
                };

                manager.userFolder.CreateUserFolder(userFolder);
            }

            await manager.SaveAsync();
        }

        public async Task DeclineRequest(string username, Guid FolderId, Guid RequestId)
        {
            var user = await userManager.FindByNameAsync(username);

            var folder = await manager.folder.GetFolder(FolderId, false);

            if (folder is null)
            {
                throw new FolderNotFoundException(FolderId);
            }

            if (!folder.OwnerId.Equals(user.Id))
            {
                throw new UnauthorizedAction("Unauthorized to perform action on folder!");
            }

            var request = await manager.request.GetRequestAsync(FolderId, RequestId, true);

            if (request is null)
            {
                throw new NotFoundException($"Request with Id {RequestId} could not be found!");
            }

            if (request.Status == RequestStatus.AwaitingAck)
            {
                request.Status = RequestStatus.Denied;
            }

            await manager.SaveAsync();
        }

        public async Task GrantWriteAccess(string granter, string requester, Guid FolderId)
        {
            var requesterObj = await userManager.FindByNameAsync(requester);

            if (requesterObj is null)
            {
                throw new NotFoundException("User not found!");
            }

            var granterObj = await userManager.FindByNameAsync(granter);

            var folder = await manager.folder.GetFolder(FolderId, false);

            if (folder is null)
            {
                throw new FolderNotFoundException(FolderId);
            }

            if (!folder.OwnerId.Equals(granterObj?.Id))
            {
                throw new UnauthorizedAction("Unauthorized to perform action on folder!");
            }

            Entities.Models.Folder baseFolder = null;

            if (folder.BaseFolderId != Guid.Empty)
            {
                baseFolder = await manager.folder.GetBaseFolder(folder.Id, trackChanges: false);
            }

            var permissions = await manager.userFolder.GetCollaboratorsForFolder(baseFolder is null ? FolderId : baseFolder.Id, true);

            var perm = permissions.Where(x => x.UserId.Equals(requesterObj.Id)).FirstOrDefault();

            if (perm is not null)
            {
                perm.Permissions = Permissions.ReadnWrite;
            }
            else
            {
                var userFolder = new UserFolders
                {
                    UserId = requesterObj.Id,
                    FolderId = FolderId,
                    Permissions = Permissions.ReadnWrite
                };

                manager.userFolder.CreateUserFolder(userFolder);
            }

            await manager.SaveAsync();
        }

        public async Task RevokeAccess(string revoker, string revoked, Guid FolderId)
        {
            var revokedObj = await userManager.FindByNameAsync(revoked);

            if (revokedObj is null)
            {
                throw new NotFoundException("User not found!");
            }

            var revokerObj = await userManager.FindByNameAsync(revoker);

            var folder = await manager.folder.GetFolder(FolderId, false);

            if (folder is null)
            {
                throw new FolderNotFoundException(FolderId);
            }

            if (!folder.OwnerId.Equals(revokerObj?.Id))
            {
                throw new UnauthorizedAction("Unauthorized to perform action on folder!");
            }

            Entities.Models.Folder baseFolder = null;

            if (folder.BaseFolderId != Guid.Empty)
            {
                baseFolder = await manager.folder.GetBaseFolder(folder.Id, trackChanges: false);
            }

            var permissions = await manager.userFolder.GetCollaboratorsForFolder(baseFolder is null ? FolderId : baseFolder.Id, true);
            var perm = permissions.Where(x => x.UserId.Equals(revokedObj.Id)).FirstOrDefault();

            if(perm is not null)
            {
                manager.userFolder.DeleteUserFolder(perm);

                await manager.SaveAsync();
            }
        }
    }
}
