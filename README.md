# MindedBanking

## Considerations
1) A user can only have one account
2) One account can belong only to a user
3) I did not differentiate between domain model and data entities but it is recommended
4) Security has been omitted but it is very important and should be added
5) The API is also serving the UI, this is not ideal but simplifies the project
6) Most of the testing has been omitted to make sure the features are completed in a timely manner, but this is not acceptable for production code
7) I leveraged the filter capability but for a good implementation of the controllers, it would be nice to have a standardized structure with Get, GetById, Patch, Delete

## Solution structure
### UI Project
The UI project is written in Angular and placed in the folder _MBApplicationUI_

### API Project
The apy project is _MB.Application.Api_, this is the entry point and uses **Minded** NuGet package (which I created), to create a Command/Query, Mediator and Decorator pattern.

### QM.Business
Projects called _QM.Business.*_ contains business logic and there is where commands, queries and corresponding handlers are placed.

### QM.Data
Projects called _QM.Data.*_ are coupled to the datasource and responsible to handle data connections. I did not use in this project the repository pattern, but in more complex scenarious, especially when dealing with legacy data sources, it is recommended.

## Build
1) Create a sql server database and update the connection string 'MindedBanking' in the MB.Application.Api>appsettings.json
2) Execute apply EF migration using Update-Database from the 'Package Manager Console'
3) Use 'ng build --output-path ..\MB.Application.Api\wwwroot\' to build the angular application which will be placed in the wwwroot folder, served by the main api