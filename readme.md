This repository contains an example project with connections to Azure App Configuration and Azure SQL Server. These connections are connected through username/password or key. Below is a description on how to change the project to use System assigned managed identity.

# Environment
Configuring the environment in Azure is done by deploying `main.bicep`. Run the following command inside the README folder:
```powershell
az deployment group create --resource-group managed-identity --template-file Deployment/main.bicep --parameters Deployment/main.bicepparam
```

After deployment copy the connection strings from the Azure App Service to the secrets.json or appsettings.json file.

# SQL
The SQL server default has the following username and password:<br/>
**Username**: mysqluser <br/>
**Password**: Mysqlpassword!<br/>

The database should first be updated by running the commands below inside the README folder.
Migration command:
```dotnetcli
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate --output-dir Models/Migrations
```