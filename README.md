# RemoteDispatcher
A .NET Core tool for managing remote Github repository_dispatch action triggering.
This tool provides an easier way of triggering the Github repository_dispatch action through an HTTP request that supports private repositories as well.

## Requirements
.NET 5.0 installed on the target machine.

## Usage

To install this tool, run the following cmd query:
```
dotnet tool install -g RemoteDispatcher
```

or install the .nupkg package and run the following query:
```
dotnet tool install dotnetsay --tool-path c:\path\to\package
```

The tool supports the following operations:

### 1. Triggering a repository_dispatch event
**Command:** `dispatch-repo`

 Triggers a Github action repository_dispatch event remotely.


| Command Property     | Descriptions                                                                                      |
|----------------------|---------------------------------------------------------------------------------------------------|
| -a\|--accept         | The content type of the request.                                                                  |
| -o\|--owner          | The owner or the organization name.                                                               |
| -r\|--repo           | The name of the target repository.                                                                |
| -e\|--event-type     | Required.A custom webhook event name.                                                             |
| -d\|--client-payload | JSON payload with extra information about the webhook event that your action or workflow may use. |
| -p\|--is-private     | The flag indicating whether the target repo id private or not.                                    |
| -f\|--data-file      | The path to the JSON file that contains the client payload data to load.                          |

RemoteDispatcher also supports private repositories. However, to use this feature, the tool requires an environment variable called `SALIH_RD_TOKEN`, which should contain a personal access token that has the workflow scope defined.

### Example

- **repo_dispatch_test.yml**
```yml
name: Workflow Dispatch Test

on:
  repository_dispatch:
    types: [fetched]

jobs:
  printInputs:
    runs-on: ${{ github.event.client_payload.runner }}
    steps:
    - run: |
        C:
        mkdir ${{ github.event.client_payload.folder_name }}
```
This file represents a Github workflow action that simply creates a folder with the specified name on the specified runner.

- **data.json**
```json
{
    "runner": "self-hosted",
    "folder_name": "MyFile"
}
```
This file contains the client payload data to sent along with the trigger request.


Now, all we need to do is running the following command to trigger the event that will start the action.
```
salih-rd dispatch-repo --owner [username/organization] --repo [repository] --event-type fetched --is-private true --data-file C:\path\to\data.json
```
