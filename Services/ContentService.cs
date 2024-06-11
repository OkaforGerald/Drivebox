using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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

            var Files = JsonSerializer.Serialize(HashFilesInDirectory(AbsolutePath));
            var hashedDirectories = JsonSerializer.Serialize(HashSubDirectories(AbsolutePath));

            await SyncFolders(Guid.Empty, user.Id, AbsolutePath);

            var folderForBackup = await manager.folder.GetFolderByName(user.Id, AbsolutePath.Split('\\')[^1], false);

            var Backup = new Backup
            {
                OwnerId = user.Id,
                FolderId = folderForBackup.Id,
                HashedContents = Files,
                HashedFolders = hashedDirectories,
                Path = AbsolutePath,
                CreatedAt = DateTime.Now
            };

            manager.backup.CreateBackup(Backup);

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
                        hashedFiles.Add(file, Convert.ToBase64String(hash));
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
                    HashedSubDirectories.Add(d, Convert.ToBase64String(hash));
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
                    IsOnLocal = true,
                    PathOnLocal = AbsolutePath,
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
                await manager.SaveAsync();

                if (Directory.GetFiles(AbsolutePath).Any())
                {
                    foreach (var file in Directory.GetFiles(AbsolutePath))
                    {
                        await UploadFileToFolderWithPath(file, Folder.Id);
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
                    IsOnLocal = true,
                    PathOnLocal = dirs,
                    CreatedAt = DateTime.Now,
                    OwnerId = ownerID
                };

                manager.folder.CreateFolder(SubFolder);

                if (Directory.GetFiles(dirs).Any())
                {
                    foreach (var file in Directory.GetFiles(dirs))
                    {
                        await UploadFileToFolderWithPath(file, SubFolder.Id);
                    }
                }

                await SyncFolders(SubFolder.Id, ownerID, dirs);
            }

        }

        public async Task BackupFolder(string username, Guid FolderId)
        {
            var user = await userManager.FindByNameAsync(username);

            var folder = await manager.folder.GetFolder(FolderId, trackChanges: true);

            if (folder is null)
            {
                throw new FolderNotFoundException(FolderId);
            }

            if (!folder.IsOnLocal)
            {
                throw new Exception("Folder isn't on your local machine!");
            }

            var backup = await manager.backup.GetBackup(folder.Id, trackChanges: true);

            if(backup is null)
            {
                throw new NotFoundException($"Backup of folder with {folder.Id} does not exist!");
            }

            var newFilesHash = HashFilesInDirectory(backup.Path);
            var newDirectoryHash = HashSubDirectories(backup.Path);

            var initialFileHash = JsonSerializer.Deserialize<Dictionary<string, string>>(backup.HashedContents);
            var initialDirectoryHash = JsonSerializer.Deserialize<Dictionary<string, string>>(backup.HashedFolders);

            bool IsChanged = false;
            //Check Folders
            foreach (var key in initialDirectoryHash.Keys)
            {
                string hash;
                var ValueExists = newDirectoryHash.TryGetValue(key, out hash);

                if (ValueExists)
                {
                    if (initialDirectoryHash[key] != hash)
                    {
                        IsChanged = true;
                        var folderToUpdate = await manager.folder.GetFolderByPath(user.Id, key, true);
                        var newKey = newDirectoryHash.Where(x => x.Value.Equals(hash)).FirstOrDefault().Key;
                        var name = newKey.Remove(0, newKey.IndexOf(newKey.Split('\\')[^1]));
                        folderToUpdate.Name = name;
                        folderToUpdate.UpdatedAt = DateTime.Now;
                    }
                }
                else
                {
                    IsChanged = true;
                    var folderToDelete = await manager.folder.GetFolderByPath(user.Id, key, true);
                    if(folderToDelete != null)
                    {
                        await manager.folder.DeleteFolder(folderToDelete);
                    }
                }
            }

            foreach (var key in newDirectoryHash.Keys.OrderBy(x => x.Split('\\').Length))
            {
                string hash;
                var ValueExists = initialDirectoryHash.TryGetValue(key, out hash);

                if (!ValueExists)
                {
                    IsChanged = true;
                    var baseFolder = await manager.folder.GetFolderByPath(user.Id, Directory.GetParent(key).FullName, trackChanges: false);

                    if (baseFolder != null)
                    {
                        var Folder = new Entities.Models.Folder
                        {
                            BaseFolderId = baseFolder.Id,
                            Name = key.Split('\\')[^1],
                            Access = Access.Private,
                            IsOnLocal = true,
                            PathOnLocal = key,
                            CreatedAt = DateTime.Now,
                            OwnerId = user.Id
                        };

                        manager.folder.CreateFolder(Folder);

                        await manager.SaveAsync();
                    }
                }
            }

            //Check files
            foreach(var key in initialFileHash.Keys)
            {
                string hash;
                var ValueExists = newFilesHash.TryGetValue(key, out hash);

                if (ValueExists)
                {
                    if (initialFileHash[key] != hash)
                    {
                        IsChanged = true;
                        var parentDirectory = Directory.GetParent(key).FullName;
                        var parentFolder = await manager.folder.GetFolderByPath(user.Id, parentDirectory, true);
                        parentFolder.UpdatedAt = DateTime.Now;
                        var contents = await manager.content.GetContentsByFolderAsync(parentFolder.Id, true);
                        var content = contents.FirstOrDefault(x => x.Id.Equals(contents));

                        await DeleteContentAsync(username, parentFolder.Id, content.Id);

                        await UploadFileToFolderWithPath(key, parentFolder.Id);
                    }
                }
                else
                {
                    IsChanged = true;
                    var parentDirectory = Directory.GetParent(key).FullName;
                    var parentFolder = await manager.folder.GetFolderByPath(user.Id, parentDirectory, true);
                    parentFolder.UpdatedAt = DateTime.Now;
                    var contents = await manager.content.GetContentsByFolderAsync(parentFolder.Id, true);
                    var content = contents.FirstOrDefault(x => x.Id.Equals(contents));

                    await DeleteContentAsync(username, parentFolder.Id, content.Id);
                    await manager.SaveAsync();
                }
            }

            foreach (var key in newFilesHash.Keys.OrderBy(x => x.Split('\\').Length))
            {
                string hash;
                var ValueExists = initialFileHash.TryGetValue(key, out hash);

                if (!ValueExists)
                {
                    IsChanged = true;
                    var parentDirectory = Directory.GetParent(key).FullName;
                    var parentFolder = await manager.folder.GetFolderByPath(user.Id, parentDirectory, true);
                    parentFolder.UpdatedAt = DateTime.Now;

                    await UploadFileToFolderWithPath(key, parentFolder.Id);
                }
            }

            backup.HashedContents = JsonSerializer.Serialize(newFilesHash);
            backup.HashedFolders = JsonSerializer.Serialize(newDirectoryHash);
            backup.UpdatedAt = DateTime.Now;

            await manager.SaveAsync();

            if (!IsChanged)
            {
                throw new Exception("No changes were found!");
            }
        }

        private async Task UploadFileToFolderWithPath(string path, Guid subFolderId)
        {
            var fileType = GetFileType(path);

            var result = fileType switch
            {
                FileType.Videos => UploadVideos(subFolderId, FilePath: path),
                FileType.Images => UploadImages(subFolderId, FilePath: path),
                _ => UploadFiles(subFolderId, FilePath: path)
            };
            var content = new Content
            {
                CreatedAt = DateTime.Now,
                FolderId = subFolderId,
                Name = Path.GetFileNameWithoutExtension(path),
                Size = File.OpenRead(path).Length,
                FileExt = Path.GetExtension(path),
                FileType = fileType,
                URL = result.Url.AbsoluteUri
            };

            manager.content.CreateContent(content);

            await manager.SaveAsync();
        }
    }
}
