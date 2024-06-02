using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Services.Contracts;

namespace Services
{
    public class ContentService : IContentService
    {
        private readonly Cloudinary cloudinary;
        private readonly IConfiguration configuration;
        private readonly IRepositoryManager manager;
        private readonly UserManager<User> userManager;

        public ContentService(IConfiguration configuration, IRepositoryManager manager, UserManager<User> userManager)
        {
            this.configuration = configuration;
            cloudinary = new Cloudinary(new Account(configuration.GetSection("Cloudinary")["CloudName"],
                configuration.GetSection("Cloudinary")["API-KEY"],
                configuration.GetSection("Cloudinary")["API-SECRET"]));
            cloudinary.Api.Secure = true;
            this.manager = manager;
            this.userManager = userManager;
        }

        public async Task CreateContent(string username, Guid FolderId, IFormFile file)
        {
            var user = await userManager.FindByNameAsync(username);

            var folder = await manager.folder.GetFolder(FolderId, trackChanges: true);

            if (folder is null)
            {
                throw new FolderNotFoundException(FolderId);
            }
            var baseFolder = await manager.folder.GetBaseFolder(folder.Id, false);

            var collaborators = await manager.userFolder.GetCollaboratorsForFolder(baseFolder.Id, false);

            if (!collaborators.Where(x => x.Permissions == Permissions.ReadnWrite).Any(x => x.UserId.Equals(user.Id)))
            {
                throw new UnauthorizedFolderException(baseFolder.Id);
            }

            var contentsInFolder = await manager.content.GetContentsByFolderAsync(FolderId, false);
            if(contentsInFolder is not null && contentsInFolder.Any(x => x.Name.Equals(file.FileName.Split('.')[0], StringComparison.CurrentCultureIgnoreCase)))
            {
                throw new Exception("Content with the same name already exists!");
            }

            var fileType = GetFileType(file);

            var result = fileType switch
            {
                FileType.Videos => UploadVideos(FolderId, file),
                FileType.Images => UploadImages(FolderId, file),
                _ => UploadFiles(FolderId, file),
            };

            folder.UpdatedAt = DateTime.Now;

            var content = new Content
            {
                CreatedAt = DateTime.Now,
                FolderId = folder.Id,
                Name = file.FileName.Remove(file.FileName.IndexOf('.')),
                Size = file.Length,
                FileExt = Path.GetExtension(file.FileName),
                FileType = fileType,
                URL = result.Url.AbsoluteUri
            };

            manager.content.CreateContent(content);
            await manager.SaveAsync();
        }

        private FileType GetFileType(IFormFile file)
        {
            List<string> ImageExts = new List<string> { ".JPG", ".JPEG", ".JPE", ".BMP", ".GIF", ".PNG" };
            List<string> VideoExts = new List<string> { ".AVI", ".MOV", ".WEBM", ".MP4", ".WMV", ".FLV", ".MKV", "AVCHD" };
            var extension = Path.GetExtension(file.FileName);

            if (ImageExts.Contains(extension.ToUpper()))
            {
                return FileType.Images;
            }else if (VideoExts.Contains(extension.ToUpper()))
            {
                return FileType.Videos;
            }
            else
            {
                return FileType.Docs;
            }
        }

        private UploadResult UploadImages(Guid FolderId, IFormFile file)
        {
            using(var stream = file.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams
                {
                    Folder = FolderId.ToString(),
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(800)
                };

                var uploadResult = cloudinary.Upload(uploadParams);
                return uploadResult;
            }
        }

        private UploadResult UploadVideos(Guid FolderId, IFormFile file)
        {
            using (var stream = file.OpenReadStream())
            {
                var uploadParams = new VideoUploadParams
                {
                    Folder = FolderId.ToString(),
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(800)
                };

                var uploadResult = cloudinary.Upload(uploadParams);
                return uploadResult;
            }
        }

        private UploadResult UploadFiles(Guid FolderId, IFormFile file)
        {
            using (var stream = file.OpenReadStream())
            {
                var uploadParams = new AutoUploadParams
                {
                    Folder = FolderId.ToString(),
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(800)
                };

                var uploadResult = cloudinary.Upload(uploadParams);
                return uploadResult;
            }
        }

        public async Task DeleteContentAsync(string username, Guid FolderId, Guid ContentId)
        {
            var user = await userManager.FindByNameAsync(username);

            var folder = await manager.folder.GetFolder(FolderId, trackChanges: true);

            if (folder is null)
            {
                throw new FolderNotFoundException(FolderId);
            }
            var baseFolder = await manager.folder.GetBaseFolder(folder.Id, false);

            var collaborators = await manager.userFolder.GetCollaboratorsForFolder(baseFolder.Id, false);

            if (!collaborators.Where(x => x.Permissions == Permissions.ReadnWrite).Any(x => x.UserId.Equals(user.Id)))
            {
                throw new UnauthorizedFolderException(baseFolder.Id);
            }

            var content = await manager.content.GetContentAsync(FolderId, ContentId, trackChanges: true);

            if(content is null)
            {
                throw new CodeNotFoundException(ContentId);
            }

            await cloudinary.DestroyAsync(new DeletionParams($"{content.URL.Split('/')[^2]}/{content.URL.Split('/')[^1].Split('.')[0]}"));

            manager.content.DeleteContent(content);

            await manager.SaveAsync();
        }
    }
}
