[ownership]: OWNERSHIP.md
[notice]: NOTICE.md
[contributing]: CONTRIBUTING.md

# Patterns Playground
An assortment of demo solutions and fun projects showcasing different technologies, patterns, and best practices.

## Polling Dashboard #1
Updating a web app dashboard and displaying the running status of jobs is a challenge. It becomes even more difficult when you need to coordinate running jobs across multiple browsers and/or users, so that the progress indicator for a job displays automatically when another user kicks off the job, and more importantly, no two users can execute the same job at the same time.

This is a hard problem, but also very common in corporate settings. This application demonstrates an approach that uses a simple polling mechanism in tandem with out-of-the-box HTTP features such as ETag and If-None-Match to efficiently query job-running status from the API without introducing the complexity of web sockets.

### Concepts Demonstrated
- [C# record types](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/record)
    - See also [this blog entry](https://mattjameschampion.com/2023/09/22/c-struct-record-class-and-record-struct-cheat-sheet/).
- ASP.NET Core
    - [Caching](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/distributed?view=aspnetcore-10.0)
    - [Dependency Injection](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-10.0)
- [Synchronization primitives](https://learn.microsoft.com/en-us/dotnet/api/system.threading.semaphoreslim?view=net-10.0)
- [Minimal APIs](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis?view=aspnetcore-10.0)
- [RxJS](https://rxjs.dev/api)
- [PrimeNG](https://primeng.org/installation)
- [Angular](https://angular.dev/overview) 21, standalone components, zoneless
- Visual Studio [Build/Packages.props](https://ryanbuening.com/posts/central-package-management/)

### Prerequisites
You need .NET 10 and Angular 21 to run this demo.
```
choco install -y visualstudio2026community
choco install -y nvm
nvm install latest
nvm use latest
npm install -g @angular/cli
```

### Running
1. Open JDS.PollingDashboard1.slnx in Visual Studio.
1. Hit F5.
1. Open a command prompt.
```
cd JDS.PollingDashboard1.UI
npm i
npm start
```
4. Open a browser and navigate to https://localhost:4200 

## Authors

* **John Jacobs** - *Initial work* - [JacobsDataSolutions](https://github.com/JacobsDataSolutions)

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

Copyright (c) 2026 Jacobs Data Solutions, LLC

Please see [notice][notice] and [ownership][ownership] for important legal information.