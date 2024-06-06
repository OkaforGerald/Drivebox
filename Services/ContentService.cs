using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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

        private FileType GetFileType(string FilePath)
        {
            List<string> ImageExts = new List<string> { ".JPG", ".JPEG", ".JPE", ".BMP", ".GIF", ".PNG" };
            List<string> VideoExts = new List<string> { ".AVI", ".MOV", ".WEBM", ".MP4", ".WMV", ".FLV", ".MKV", "AVCHD" };
            var extension = Path.GetExtension(FilePath);

            if (ImageExts.Contains(extension.ToUpper()))
            {
                return FileType.Images;
            }
            else if (VideoExts.Contains(extension.ToUpper()))
            {
                return FileType.Videos;
            }
            else
            {
                return FileType.Docs;
            }
        }

        private UploadResult UploadImages(Guid FolderId, IFormFile file = null, string FilePath = null)
        {
            if(FilePath is not null)
            {
                using(var stream = File.OpenRead(FilePath))
                {
                    var uploadParams = new ImageUploadParams
                    {
                        Folder = FolderId.ToString(),
                        File = new FileDescription(Path.GetFileName(FilePath), stream),
                        Transformation = new Transformation().Height(500).Width(800)
                    };

                    var uploadResult = cloudinary.Upload(uploadParams);
                    return uploadResult;
                }
            }
            else
            {
                using (var stream = file.OpenReadStream())
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
        }

        private UploadResult UploadVideos(Guid FolderId, IFormFile file = null, string FilePath = null)
        {
            if(FilePath is not null)
            {
                using (var stream = File.OpenRead(FilePath))
                {
                    var uploadParams = new VideoUploadParams
                    {
                        Folder = FolderId.ToString(),
                        File = new FileDescription(Path.GetFileName(FilePath), stream),
                        Transformation = new Transformation().Height(500).Width(800)
                    };

                    var uploadResult = cloudinary.Upload(uploadParams);
                    return uploadResult;
                }
            }
            else
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
        }

        private UploadResult UploadFiles(Guid FolderId, IFormFile file = null, string FilePath = null)
        {
            if(FilePath is not null)
            {
                using (var stream = File.OpenRead(FilePath))
                {
                    var uploadParams = new AutoUploadParams
                    {
                        Folder = FolderId.ToString(),
                        File = new FileDescription(Path.GetFileName(FilePath), stream),
                        Transformation = new Transformation().Height(500).Width(800)
                    };

                    var uploadResult = cloudinary.Upload(uploadParams);
                    return uploadResult;
                }
            }
            else
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

        public async Task SyncLocalFolder(string username, string AbsolutePath)
        {
            var user = await userManager.FindByNameAsync(username);
            var PathExists = Directory.Exists(AbsolutePath);

            if (!PathExists)
            {
                throw new NotFoundException("Path couldn't be found");
            }

            var Files = HashFilesInDirectory(AbsolutePath);
            var hashedDirectories = HashSubDirectories(AbsolutePath);

            await SyncFolders(Guid.Empty, user.Id, AbsolutePath);
            await manager.SaveAsync();

        }

        private Dictionary<string, string> HashFilesInDirectory(string AbsolutePath)
        {
            Dictionary<string, string> hashedFiles = new Dictionary<string, string>();

            if (Directory.GetFiles(AbsolutePath) is null)
            {
                return hashedFiles;
            }

            var files = Directory.GetFiles(AbsolutePath, "*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                using (var algo = SHA256.Create())
                {
                    using (var stream = File.OpenRead(file))
                    {
                        var hash = algo.ComputeHash(stream);
                        hashedFiles.Add(file.Remove(0, file.IndexOf(AbsolutePath.Split('\\')[^1])), Convert.ToBase64String(hash));
                    }
                }
            }
            return hashedFiles;
        }

        private Dictionary<string, string> HashSubDirectories(string AbsolutePath)
        {
            Dictionary<string, string> HashedSubDirectories = new Dictionary<string, string>();

            if (Directory.GetDirectories(AbsolutePath) is null)
            {
                return HashedSubDirectories;
            }

            var directories = Directory.GetDirectories(AbsolutePath, "*", SearchOption.AllDirectories);

            foreach (var d in directories)
            {
                using (var algo = SHA256.Create())
                {
                    var hash = algo.ComputeHash(Encoding.UTF8.GetBytes(d));
                    HashedSubDirectories.Add(d.Remove(0, d.IndexOf(AbsolutePath.Split('\\')[^1])), Convert.ToBase64String(hash));
                }
            }
            return HashedSubDirectories;
        }

        private async Task SyncFolders(Guid BaseFolderId, string ownerID, string AbsolutePath)
        {
            var PathExists = Directory.Exists(AbsolutePath);

            if (!PathExists)
            {
                throw new NotFoundException("Path couldn't be found");
            }

            var childFolders = await manager.folder.GetChildFolders(BaseFolderId, ownerID, trackChanges: false);

            var FolderExists = childFolders.Any(x => x.Name.Equals(AbsolutePath.Split('\\')[^1]));

            if (FolderExists)
            {
                throw new Exception("Folder already exists!");
            }

            Guid FolderId = BaseFolderId;
            if (BaseFolderId.Equals(Guid.Empty))
            {
                var Folder = new Entities.Models.Folder
                {
                    BaseFolderId = BaseFolderId,
                    Name = AbsolutePath.Split('\\')[^1],
                    Access = Access.Private,
                    CreatedAt = DateTime.Now,
                    OwnerId = ownerID
                };

                manager.folder.CreateFolder(Folder);

                var userFolder = new UserFolders
                {
                    UserId = ownerID,
                    FolderId = Folder.Id,
                    Permissions = Permissions.ReadnWrite
                };

                manager.userFolder.CreateUserFolder(userFolder);

                if (Directory.GetFiles(AbsolutePath).Any())
                {
                    foreach (var file in Directory.GetFiles(AbsolutePath))
                    {
                        var fileType = GetFileType(file);

                        var result = fileType switch
                        {
                            FileType.Videos => UploadVideos(Folder.Id, FilePath: file),
                            FileType.Images => UploadImages(Folder.Id, FilePath: file),
                            _ => UploadFiles(Folder.Id, FilePath: file),
                        };
                        var content = new Content
                        {
                            CreatedAt = DateTime.Now,
                            FolderId = Folder.Id,
                            Name = Path.GetFileNameWithoutExtension(file),
                            Size = File.OpenRead(file).Length,
                            FileExt = Path.GetExtension(file),
                            FileType = fileType,
                            URL = result.Url.AbsoluteUri
                        };

                        manager.content.CreateContent(content);
                    }
                }
                FolderId = Folder.Id;
            }

            var topDirectories = Directory.GetDirectories(AbsolutePath, "*", SearchOption.TopDirectoryOnly);

            foreach (var dirs in topDirectories)
            {
                var SubFolder = new Entities.Models.Folder
                {
                    BaseFolderId = FolderId,
                    Name = dirs.Split('\\')[^1],
                    Access = Access.Private,
                    CreatedAt = DateTime.Now,
                    OwnerId = ownerID
                };

                manager.folder.CreateFolder(SubFolder);

                if (Directory.GetFiles(dirs).Any())
                {
                    foreach (var file in Directory.GetFiles(dirs))
                    {
                        var fileType = GetFileType(file);

                        var result = fileType switch
                        {
                            FileType.Videos => UploadVideos(SubFolder.Id, FilePath: file),
                            FileType.Images => UploadImages(SubFolder.Id, FilePath: file),
                            _ => UploadFiles(SubFolder.Id, FilePath: file),
                        };
                        var content = new Content
                        {
                            CreatedAt = DateTime.Now,
                            FolderId = SubFolder.Id,
                            Name = Path.GetFileNameWithoutExtension(file),
                            Size = File.OpenRead(file).Length,
                            FileExt = Path.GetExtension(file),
                            FileType = fileType,
                            URL = result.Url.AbsoluteUri
                        };

                        manager.content.CreateContent(content);

                        await manager.SaveAsync();
                    }
                }

                await SyncFolders(SubFolder.Id, ownerID, dirs);
            }

        }
    }
}
