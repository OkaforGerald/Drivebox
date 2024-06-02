using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Contracts;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Services.Contracts;

namespace Services
{
    public class ServiceManager : IServiceManager
    {
        private readonly Lazy<IAuthService> _authService;
        private readonly Lazy<IFolderService> _folderService;
        private readonly Lazy<IContentService> _contentService;
        private readonly Lazy<IRequestService> _requestService;
        public ServiceManager(UserManager<User> userManager, IMapper mapper, IConfiguration configuration, IRepositoryManager manager)
        {
            _authService = new Lazy<IAuthService>(new AuthService(userManager, mapper, configuration));
            _folderService = new Lazy<IFolderService>(new FolderService(manager, userManager, mapper));
            _contentService = new Lazy<IContentService>(new ContentService(configuration, manager, userManager));
            _requestService = new Lazy<IRequestService>(new RequestService(manager, userManager));
        }

        public IAuthService AuthService => _authService.Value;

        public IFolderService FolderService => _folderService.Value;

        public IContentService ContentService => _contentService.Value;

        public IRequestService RequestService => _requestService.Value;
    }
}
