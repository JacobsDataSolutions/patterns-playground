[ownership]: OWNERSHIP.md
[notice]: NOTICE.md

# Patterns Playground
An assortment of demo solutions and fun projects showcasing different technologies, patterns, and best-practices.

## Polling Dashboard #1
Updating a web app dashboard and displaying the running status of jobs is a challenging problem. It becomes even more difficult when you need to coordinate running jobs across multiple browsers and/or users, so that the progress indicator for a job displays automatically when another user kicks off the job, and more importantly, no two users can execute the same job at the same time.

This is a hard problem but also very common in corporate settings. This application demonstrates an approach that uses a simple polling mechanism in tandem with out-of-the-box HTTP features such as ETag and If-None-Match to efficiently query job-running status from the API without introducing the complexity of web sockets.

### Concepts Demonstrated
- C# record types
- ASP.NET Core caching
- Synchronization primitives
- Minimal APIs
- RxJS
- PrimeNG
- Angular 21, standalone components, zoneless

Copyright (c) 2026 Jacobs Data Solutions, LLC

Please see [notice][notice] and [ownership][ownership] for important legal information.