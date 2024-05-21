using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using SharedAPI.DataTransfer;

namespace Services
{
    public class FolderService
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

            var newFolder = new Folder
            {
                Name = folder.Name,
                Access = folder.Access,
                CreatedAt = DateTime.Now,
                OwnerId = user?.Id
            };

            //Create Permissions

            manager.folder.CreateFolder(newFolder);
            await manager.SaveAsync();
        }

        public async Task DeleteFolderAsync(string username, Guid FolderId)
        {
            var folder = await manager.folder.GetFolder(FolderId, trackChanges: false);

            if(folder is null)
            {
                throw new FolderNotFoundException(FolderId);
            }

            manager.folder.DeleteFolder(folder);
            await manager.SaveAsync();
        }

    }
}
