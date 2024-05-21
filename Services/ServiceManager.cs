using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Services.Contracts;

namespace Services
{
    public class ServiceManager : IServiceManager
    {
        private readonly Lazy<IAuthService> _authService;
        public ServiceManager(UserManager<User> userManager, IMapper mapper, IConfiguration configuration)
        {
            _authService = new Lazy<IAuthService>(new AuthService(userManager, mapper, configuration));
        }

        public IAuthService AuthService => _authService.Value;
    }
}
