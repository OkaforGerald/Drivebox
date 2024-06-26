﻿using AutoMapper;
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
                .ForMember(x => x.Size, src => src.MapFrom(x => GetFileSize(x.Size)))
                .ForMember(x => x.FileType, src => src.MapFrom(x => x.FileType.ToString()));
        }

        public string GetFileSize(long Size)
        {
            string[] denomination = { "B", "KB", "MB", "GB" };

            int divisionCount = 0;

            while (true)
            {
                if (Size < 1023)
                {
                    return $"{Size} {denomination[divisionCount]}";
                }
                else
                {
                    Size = Size / 1023;
                    divisionCount++;
                }
            }
        }
    }
}
