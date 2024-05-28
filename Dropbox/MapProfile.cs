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

            CreateMap<Folder, FolderDto>()
                .ForMember(x => x.Owner, src => src.MapFrom(x => x.Owner.UserName))
                .ForMember(x => x.Access, src => src.MapFrom(x => x.Access.ToString()));

            CreateMap<Content, ContentDto>()
                .ForMember(x => x.Size, src => src.MapFrom(x => $"{(double) x.Size / 1_046_529} MB"))
                .ForMember(x => x.FileType, src => src.MapFrom(x => x.FileType.ToString()));
        }
    }
}
