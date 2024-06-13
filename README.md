# Drivebox

## Overview
Drivebox is a Dropbox Clone from the John Crickett's "Build Your Own" series built using C# .NET Web API and SQL Server for the database.
## Table of Contents

- [Installation](#installation)
- [Usage](#usage)
- [Features](#features)
- [Sample Endpoints](#endpoints)
- [Contributing](#contributing)

## Installation

To get started with Dropbox, follow these steps:

```bash
# Clone the repository
git clone https://github.com/OkaforGerald/Drivebox.git

# Navigate to the project directory
cd Drivebox

# Install dependencies
dotnet restore
```
## Usage

After installing the dependencies, you can build and run the project:

```bash
# Build the project
dotnet build

# Run the project
dotnet run
```

## Features
1. Authentication and Authorization:
    - Secure user authentication and authorization
    - Token-based authentication with JWT and refresh token pairs
2. Folder and File Management:
    - Create, rename, and delete folders and files
    - Upload and download files
3. Permission Management:
    - Grant and revoke read and write access to users
    - Inheritance permissions for folder hierarchies
4. File Sharing:
    - Share files and folders with users
5. Search and Filter:
    - Search files and folders by name and content
    - Filter files by type and modified date
6. Backup and Sync:
    - Delta Synchronization of files/folders on local machine using hash comparisons transferring only the changes (deltas) between the local and online versions, as opposed to reuploading the entire folder

## Endpoints

- POST /api/folders: Creates a new folder or subfolder(if base folder id is provided)
```curl
curl -X POST https://localhost:7264/api/folders
```
Body:
```json
{
  //"baseFolderId": "3fa85f64-5717-4562-b3fc-2c963f66afa6" if NewFolder is a subfolder
  "name": "NewFolder",
  "access": 100
}
```
Response:
```json
{
  "isSuccessful": true,
  "statusCode": 200,
  "data": "Folder created successfully!",
  "errors": null
}
```

- GET /api/folders: Gets all folders at the top of the folder hierachy i.e not a child folder to any folder.(Searching, Filtering and Sorting done with the query parameters)
```curl
curl -X GET https://localhost:7264/api/Folders?SearchTerm=Drop&OrderBy=Name%20asc&FolderType=Private
```
Response:
```json
{
  "isSuccessful": true,
  "statusCode": 200,
  "data": [
    {
      "id": "7d08c25e-6c81-43cc-e69a-08dc8adb16a1",
      "baseFolderId": "00000000-0000-0000-0000-000000000000",
      "owner": "badgateway5xx",
      "name": "DropboxClone",
      "access": "Private",
      "createdAt": "2024-06-12T13:27:56.1628138",
      "updatedAt": "0001-01-01T00:00:00"
    }
  ],
  "errors": null
}
```

- GET /api/folders/{Id}: Gets a specific folder by Id along with it's subfolders and files.(Searching, Filtering and Sorting done with the query parameters)
```curl
curl -X GET https://localhost:7264/api/Folders/7d08c25e-6c81-43cc-e69a-08dc8adb16a1
```
Response:
```json
{
  "isSuccessful": true,
  "statusCode": 200,
  "data": {
    "id": "7d08c25e-6c81-43cc-e69a-08dc8adb16a1",
    "baseFolderId": "00000000-0000-0000-0000-000000000000",
    "owner": "badgateway5xx",
    "name": "DropboxClone",
    "access": "Private",
    "collaborators": [
      {
        "userName": "badgateway5xx",
        "permissions": "ReadnWrite"
      }
    ],
    "folders": [
      {
        "id": "988f49a4-4ec2-4b9c-e69b-08dc8adb16a1",
        "baseFolderId": "7d08c25e-6c81-43cc-e69a-08dc8adb16a1",
        "owner": null,
        "name": "Docs",
        "access": "Private",
        "createdAt": "2024-06-12T13:28:04.5987954",
        "updatedAt": "0001-01-01T00:00:00"
      },
      {
        "id": "917ecf45-d62e-4bad-e69c-08dc8adb16a1",
        "baseFolderId": "7d08c25e-6c81-43cc-e69a-08dc8adb16a1",
        "owner": null,
        "name": "Images",
        "access": "Private",
        "createdAt": "2024-06-12T13:28:05.5230005",
        "updatedAt": "0001-01-01T00:00:00"
      },
      {
        "id": "a6e830a1-fcba-4027-e69d-08dc8adb16a1",
        "baseFolderId": "7d08c25e-6c81-43cc-e69a-08dc8adb16a1",
        "owner": null,
        "name": "Videos",
        "access": "Private",
        "createdAt": "2024-06-12T13:28:06.7482283",
        "updatedAt": "0001-01-01T00:00:00"
      }
    ],
    "contents": [
      {
        "id": "97545bce-d639-42ef-8136-08dc8adb1b8f",
        "name": "Onyeka Modelling Assignment",
        "fileExt": ".png",
        "fileType": "Images",
        "size": "48 KB",
        "url": "http://res.cloudinary.com/dsfkvfq8u/image/upload/v1718195285/7d08c25e-6c81-43cc-e69a-08dc8adb16a1/zvbnyahmbrs8jm8yaql9.png",
        "createdAt": "2024-06-12T13:28:04.4532208"
      }
    ],
    "createdAt": "2024-06-12T13:27:56.1628138",
    "updatedAt": "0001-01-01T00:00:00"
  },
  "errors": null
}
```

- POST /api/folders/sync: Syncs a directory present on local to a corresponding folder in the user's drivebox account
```curl
curl -X POST https://localhost:7264/api/Folders/sync?path=C%3A%5CUsers%5CPublic%5CDocuments%5CSyncTest
```
Response:
```json
{
  "isSuccessful": true,
  "statusCode": 200,
  "data": "Folder synced successfully!",
  "errors": null
}
```
- PUT /api/folders/{Id}/backup: Checks for changes in a "synced" folder since last backup and puts those changes into effect
```curl
curl -X PUT https://localhost:7264/api/Folders/13dc3e58-6f5a-4314-ed75-08dc8bd9a2e1/backup
```
Response:
```json
{
  "IsSuccessful": true,
  "StatusCode": 200,
  "Data": null,
  "Errors": [
    "No changes were found!"
  ]
}
```
- DELETE /api/folders/{Id}/contents: Deletes a folder as well as it's subfolders and contents
```curl
curl -X DELETE https://localhost:7264/api/folders/4c6b9432-ac4b-4848-fe15-08dc8bbb35b7
```
Response:
```json
{
  "isSuccessful": true,
  "statusCode": 200,
  "data": "Folder deleted successfully!",
  "errors": null
}
```

- POST /api/folders/{Id}/contents: Uploads a file to a folder you own
```curl
curl -X POST https://localhost:7264/api/folders/4c6b9432-ac4b-4848-fe15-08dc8bbb35b7/Contents
```
Response:
```json
{
  "isSuccessful": true,
  "statusCode": 200,
  "data": "File Created Successfully!",
  "errors": null
}
```

- DELETE /api/folders/{Id}/contents/{Id}: Deletes a file under a folder
```curl
curl -X DELETE https://localhost:7264/api/folders/4c6b9432-ac4b-4848-fe15-08dc8bbb35b7/Contents/60b8c25e-6c81-43cc-e69a-08dc8adb16a1
```
Response:
```json
{
  "isSuccessful": true,
  "statusCode": 200,
  "data": "File deleted successfully!",
  "errors": null
}
```

- POST /api/folders/{Id}/requests: Make a request to access a folder, usually if folder is private
```curl
curl -X POST https://localhost:7264/api/folders/7d08c25e-6c81-43cc-e69a-08dc8adb16a1/Requests
```
Response:
```json
{
  "isSuccessful": true,
  "statusCode": 200,
  "data": "Request made successfully!",
  "errors": null
}
```

- GET /api/folders/{Id}/requests: Get all requests for a folder you own
```curl
curl -X GET https://localhost:7264/api/folders/7d08c25e-6c81-43cc-e69a-08dc8adb16a1/Requests
```
Response:
```json
{
  "isSuccessful": true,
  "statusCode": 200,
  "data": [
    {
      "id": "6671619a-6e7f-45f8-a55c-08dc8bdeb606",
      "requester": "vanell0pee",
      "folderId": "7d08c25e-6c81-43cc-e69a-08dc8adb16a1",
      "status": 300,//Awaiting Acknowledgement
      "createdAt": "2024-06-13T20:26:23.2565325"
    }
  ],
  "errors": null
}
```

- PUT /api/folders/{Id}/requests/acknowledge: Accepts a request on a folder you own and gives the requester read-only access to the folder and its subfolders/contents
```curl
curl -X PUT https://localhost:7264/api/folders/7d08c25e-6c81-43cc-e69a-08dc8adb16a1/Requests/acknowledge?RequestId=6671619a-6e7f-45f8-a55c-08dc8bdeb606
```
Response:
```json
{
  "isSuccessful": true,
  "statusCode": 200,
  "data": "Request accepted successfully!",
  "errors": null
}
```

- POST /api/folders/{Id}/permissions/{username}/grant-write: Grants read and write access to a user of your choice to a folder you own
```curl
curl -X POST https://localhost:7264/api/folders/7d08c25e-6c81-43cc-e69a-08dc8adb16a1/Permissions/vanell0pee/grant-write
```
Response:
```json
{
  "isSuccessful": true,
  "statusCode": 200,
  "data": "Write access granted!",
  "errors": null
}
```

- POST /api/folders/{Id}/permissions/{username}/revoke: Revokes a user's access to a folder you own
```curl
curl -X POST https://localhost:7264/api/folders/7d08c25e-6c81-43cc-e69a-08dc8adb16a1/Permissions/vanell0pee/revoke
```
Response:
```json
{
  "isSuccessful": true,
  "statusCode": 200,
  "data": "Access revoked!",
  "errors": null
}
```

## Contributing

Contributions are welcome! If you have any suggestions, bug reports, or feature requests, please open an issue or submit a pull request.
