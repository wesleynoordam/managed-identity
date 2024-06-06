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

# Using managed identity
There are a couple of things which need to be done in order to start using managed identity.
1. First enable the **System Assigned** managed identity on the app service. You can do this in the Portal by going to the appservice and identity. The other way is through bicep. Enable managed identity with the following code block in the app service resource:
    ```bicep
    identity: {
        type: 'SystemAssigned'
    }
    ```
2. a) Assign roles to the newly added identity. **Azure SQL is an exception for this process as there is no support voor role assignments from the portal or bicep. See step 2b for SQL.** This can also be done in the portal by going to the specific resource and adding the role through Acces control (IAM).
<br/><br/>b) Assigning roles to Azure SQL Server is done with queries. *Only the server admin may be set through bicep or the portal.* To grant access for identities run the following queries:
    ```sql
    CREATE USER [IDENTITY_NAME] FROM EXTERNAL PROVIDER
    ALTER ROLE db_datareader ADD MEMBER [IDENTITY_NAME]
    ```

3. Use the identity in code to connect to the resources. This is different for each resource.
   
   **App Configuration:**
    ```csharp
    // appConfigurationUri should contain the uri to the App Configuration resource.
    var appConfigurationUri = builder.Configuration.GetValue<string>("appConfigurationUri");
        builder.Configuration.AddAzureAppConfiguration(options
            => options.Connect(
                new Uri(appConfigurationUri!),
                new DefaultAzureCredential()));
    ```

    **SQL:**<br/>
    The only change that is needed is changing the connection string by removing `User Id` and `Password` and adding `Authentication=Active Directory Default`.

# 'Hacking' managed identity
Managed Identity isn't completely 'safe'. The landscape of oauth/openid still requires tokens to be sent to the receiving party. This is no exception for managed identity, thus there is the possibility to hijack a token and use it to your advantage. Below are curl commands which you can run on a Azure App Service SSH session. The values of $IDENTITY_ENDPOINT and $IDENTITY_HEADER are found in the environment variables of the app service.

```bash
curl "$IDENTITY_ENDPOINT?resource=https://appcs-man-iden.azconfig.io&api-version=2017-09-01" -H secret:$IDENTITY_HEADER
curl "$IDENTITY_ENDPOINT?resource=https://vault.azure.net&api-version=2017-09-01" -H secret:$IDENTITY_HEADER
curl "$IDENTITY_ENDPOINT?resource=https://management.azure.com/&api-version=2017-09-01" -H secret:$IDENTITY_HEADER
```
