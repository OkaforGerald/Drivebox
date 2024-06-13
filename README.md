# Drivebox

## Overview
Drivebox is a Dropbox Clone from the John Crickett's "Build Your Own" series built using C# .NET Web API and SQL Server for the database.
## Table of Contents

- [Installation](#Installation)
- [Usage](#usage)
- [Features](#features)
- [Sample Endpoints](#SampleEndpoints)
- [Contributing](#contributing)

## Installation

To get started with Dropbox, follow these steps:

```bash
# Clone the repository
git clone https://github.com/OkaforGerald/Dropbox.git

# Navigate to the project directory
cd Dropbox

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

## Sample Endpoints


## Contributing

Contributions are welcome! If you have any suggestions, bug reports, or feature requests, please open an issue or submit a pull request.
