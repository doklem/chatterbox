# Chatterbox

> A developer's playground for WPF and .NET Core 3 in the form of a real time "hello world": an instant messenger.

![alt text](https://github.com/doklem/chatterbox/wiki/images/sample-chat.gif "Sample chat")

## Table of Contents

* [Features](#features)
* [Project Scope](#project-scope)
* [Build](#build)
  * [Building the Client on Windows](#building-the-client-on-windows)
  * [Building the Server on Windows](#building-the-server-on-windows)
* [Disclaimer](#disclaimer)
* [License](#license)

## Features

Chatterbox is a basic instant messenger. It consists of two elements:

* A desktop client written with [Windows Presentation Foundation (WPF)](https://docs.microsoft.com/en-us/visualstudio/designers/getting-started-with-wpf). Multiple instances can run at the same time.
* A command line server written with [ASP.NET Core Web Host](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/web-host). Only one instance can be executed.

The application shares a single chat with all connected clients. The server keeps every message in a database to support late joins. [ASP.NET Core SignalR](https://docs.microsoft.com/en-us/aspnet/core/signalr/introduction) is used for the communication between the clients and the server.

## Project Scope

The main aim of this endeavour was to play around in a real time scenario with WPF and [.NET Core](https://dotnet.microsoft.com/). Such a setup was made possible a few months ago with the .NET Core 3.0 release. In particular, the usage of the following .NET Core features within WPF where of interest:

* [Dependency injection](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection)
* [.NET Generic Host](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host)

On top of that, the project offered the possibility to get to know the [dependency diagram validation](https://docs.microsoft.com/en-us/visualstudio/modeling/validate-code-with-layer-diagrams) feature of Visual Studio as explained in this tutorial.

[![alt text](http://img.youtube.com/vi/SL0dg4f6LQg/0.jpg "Validate your architecture dependencies in Real-time with Visual Studio 2017")](https://www.youtube.com/watch?v=SL0dg4f6LQg)

To keep the playground simple, the system was developed running on a single host, but little effort would be required to run the application with [Internet Information Services (IIS)](https://en.wikipedia.org/wiki/Internet_Information_Services) and a database server.

## Build

Both elements of the solution target the [.NET Core 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1) runtime, so make sure you have it's SDK installed.

### Building the Client on Windows

Run the following statement within the solution's root folder in your command line.

```Shell
    dotnet publish -r win-x64 -c Release --self-contained -o publish/client Chatterbox.Client/Chatterbox.Client.csproj /p:PublishSingleFile=true
```

This produces a self contained version of the client, which will be located in the folder _publish/client_.

### Building the Server on Windows

Run the following statement within the solution's root folder in your command line.

```Shell
    dotnet publish -r win-x64 -c Release --self-contained -o publish/server Chatterbox.Server/Chatterbox.Server.csproj /p:PublishSingleFile=true
```

This produces a self contained version of the server, which will be located in the folder _publish/server_.

## Disclaimer

**This project serves only as a playground. It was written in a short amount of time and its scope is very limited and it is not intended for any usage within a real production environment!**

Since it is a sample application, many important features are missing. For example:

* Private- and group-chats
* Authentication
* Authorization
* End-to-end encryption
* Scalability
* Internalization
* Etc.

## License

* [MIT license](LICENSE)
* Copyright 2020 © Dominic Klemm
