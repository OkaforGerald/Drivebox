using AutoMapper;
using Entities.Models;
using SharedAPI.DataTransfer;

namespace Dropbox
{
    public class MapProfile : Profile
    {
        public MapProfile()
        {
            CreateMap<RegisterUserDto, User>();
        }
    }
}
