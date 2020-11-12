# RemoteDispatcher
A .NET Core tool for managing remote Github workflow_dispatch action triggering.
This tool provides an easier way of triggering the Github workflow_dispatch action through an HTTP request that supports private repositories as well.

## Requirements
.NET 5.0 installed on the target machine.

## Usage

To install this tool, run the following cmd query:
```
dotnet tool install -g RemoteDispatcher
```

or install the .nuget package and run the following query:
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
