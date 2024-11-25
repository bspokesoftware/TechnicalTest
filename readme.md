## BSPOKE Software Technical Assessment

This is a technical assessment for a software developer.

### Scenario

We have been approached by a startup to create a proof of concept for a banking system that will store customers, their bank accounts, and transfers. The startup has an existing mobile app in place, but wants to modernise their backend, moving into Microsoft’s Azure cloud with ASP.NET Core. They wish to use .NET 9, Entity Framework Core and connect to a SQL database. For development purposes, the startup wants to see a proof of concept using SQLite.

The company is looking to invest heavily into software and believe this project will form a backbone to the next decade of their business. Take this into consideration when forming an opinion of how and where to structure code. A future team of developers may work on the system at the same time, which will need architecture that lends itself to a team dynamic.

### Minimum requirements for this project

- Customers can be added, removed and edited storing their name, date of birth and a daily transfer limit
- Bank accounts are linked to a customer and can be added, edited, frozen and removed.
- It is possible for a customer to have no bank accounts
- Customers should be able to perform transfers between their own accounts. Transfers are logged by date and time, amount transferred and an optional reference provided by the customer
- Transfers should be verified according to the customer’s daily limit
- Further, transfers should be verified according to the originating and receiving bank account's frozen status

### Technical specification

This project requires the relevant .NET SDK. The solution `TechnicalTest.sln` contains two projects `TechnicalTest.API` and `TechnicalTest.Data`. The API project is the executable web project. The Data project is a class library containing all DAL-relevant infrastructure.

You will also find a `clean.ps1` script, which will irreversibly remove the `bin` and `obj` artifact folders .NET uses at compile and runtime.

The API project has Entity Framework Core 8 and Swagger UI installed. Launching the project will execute migrations automatically, creating the database and take you to the Swagger UI where several endpoints have been predefined for you. These are not exhaustive, and you will need to add more. Please feel free to modify endpoints as you see fit.

Any schema changes you will make need to be reflected in migrations, using the EF Core CLI to generate a new migration. You will find an initial, existing, migration as part of the Data project.

**This project makes use of .NET minimal APIs. If you wish to switch to controllers, please do so.**

**We stress this is only a backend API assessment. This is not a frontend assessment. We do not expect a frontend.**

The frontend we will use to test is Swagger UI. 

### Reasonable assumptions

You are safe to make any assumptions you please about security, functionality that hasn’t been explicitly mentioned and infrastructure, i.e., how this API may be connected to by a consumer. If you make fundamental assumptions that dictate your architecture of this system, please do make a note these to us when you send this assessment back to us.

## Submission

Submission can be done via a Git repository, forking this repository or downloading and zipping the contents of the source in an email to the person liaising with you. If you choose to manually zip source, please use the `clean.ps1` script to remove .NET artifacts or manually delete the `bin` and `obj` folders from the relevant project directories.