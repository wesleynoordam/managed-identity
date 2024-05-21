## SQL
Username: mysqluser
Password: Mysqlpassword!

Migration command:
```cli
dotnet ef migrations add InitialCreate --output-dir Models/Migrations
```

## Deployment
```powershell
az deployment group create --resource-group managed-identity --template-file Deployment/main.bicep --parameters Deployment/main.bicepparam
```