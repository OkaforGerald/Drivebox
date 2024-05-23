using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet.Actions;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Services.Contracts;
using SharedAPI.DataTransfer;
using SharedAPI.RequestFeatures;

namespace Services
{
    public class FolderService : IFolderService
    {
        private readonly IRepositoryManager manager;
        private readonly UserManager<User> userManager;
        private readonly IMapper mapper;

        public FolderService(IRepositoryManager manager, UserManager<User> userManager, IMapper mapper)
        {
            this.manager = manager;
            this.userManager = userManager;
            this.mapper = mapper;
        }

        public async Task CreateFolderAsync(string username, CreateFolderDto folder)
        {
            var user = await userManager.FindByNameAsync(username);

            var folders = await manager.folder.GetFoldersByUser(user.Id, trackChanges: false);
            var FolderExists = folders.Any(x => x.Name.Equals(folder.Name));

            if (FolderExists)
            {
                throw new Exception("Folder already exists!");
            }

            var newFolder = new Entities.Models.Folder
            {
                Name = folder.Name,
                Access = folder.Access,
                CreatedAt = DateTime.Now,
                OwnerId = user?.Id
            };

            manager.folder.CreateFolder(newFolder);

            var userFolder = new UserFolders
            {
                UserId = user?.Id,
                FolderId = newFolder.Id,
                Permissions = Permissions.ReadnWrite
            };

            manager.userFolder.CreateUserFolder(userFolder);
            
            await manager.SaveAsync();
        }

        public async Task DeleteFolderAsync(string username, Guid FolderId)
        {
            var user = await userManager.FindByNameAsync(username);

            var folder = await manager.folder.GetFolder(FolderId, trackChanges: true);

            if(folder is null || folder.OwnerId != user?.Id)
            {
                throw new FolderNotFoundException(FolderId);
            }

            manager.folder.DeleteFolder(folder);
            await manager.SaveAsync();
        }

        public async Task<List<FolderDto>> GetFoldersForUsersAsync(string username, RequestParameters parameters)
        {
            var user = await userManager.FindByNameAsync(username);

            var folders = await manager.folder.GetFoldersByUser(user.Id, parameters, trackChanges: false);

            var response = mapper.Map<List<FolderDto>>(folders);

            return response;
        }

        public async Task<FolderDto> GetFolderAsync(string username, Guid Id)
        {
            var user = await userManager.FindByNameAsync(username);

            var folder = await manager.folder.GetFolder(Id, trackChanges: false);

            var permissions = await manager.userFolder.GetCollaboratorsForFolder(Id, false);

            var collaborators = permissions.Select(x => x.UserId)
                .ToList();

            if (folder is null)
            {
                throw new FolderNotFoundException(Id);
            }

            if(folder.Access == Access.Private && collaborators.Any(x => x.Equals(user?.Id)))
            {
                throw new UnauthorizedFolderException(Id);
            }

            var response = mapper.Map<FolderDto>(folder);

            return response;
        }
    }
}
