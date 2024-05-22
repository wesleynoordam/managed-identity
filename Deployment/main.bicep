param webAppName string
param sku string
param linuxFxVersion string
param location string = resourceGroup().location
param sqlUserId string

param username string
param userSid string

@secure()
param sqlPassword string

var appConfigurationDataReaderRole = '516239f1-63e1-4d78-a4de-a74fb236a071'

resource appServicePlan 'Microsoft.Web/serverfarms@2020-06-01' = {
  name: toLower('asp-${webAppName}')
  location: location
  properties: {
    reserved: true
  }
  sku: {
    name: sku
  }
  kind: 'linux'
}

resource appService 'Microsoft.Web/sites@2020-06-01' = {
  name: toLower('app-${webAppName}')
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      linuxFxVersion: linuxFxVersion
      connectionStrings: [
        {
          name: 'appConfiguration'
          connectionString: appConfigStore.listKeys().value[0].connectionString
        }
        {
          name: 'sqlServer'
          connectionString: 'Server=tcp:${sqlServer.name}${environment().suffixes.sqlServerHostname},1433;Initial Catalog=${sqlDb.name};Persist Security Info=False;User ID=${sqlUserId};Password=${sqlPassword};Connection Timeout=30;'
        }
      ]
    }
  }
}

resource sqlServer 'Microsoft.Sql/servers@2022-05-01-preview' = {
  name: toLower('sql-${webAppName}')
  location: 'eastus'
  properties: {
    administratorLogin: sqlUserId
    administratorLoginPassword: sqlPassword
  }

  resource allowAllWindowsAzureIps 'firewallRules' = {
    name: 'allowAllWindowsAzureIps'
    properties: {
      startIpAddress: '0.0.0.0'
      endIpAddress: '0.0.0.0'
    }
  }

  resource admin 'administrators' = {
    name: 'ActiveDirectory'
    properties: {
      administratorType: 'ActiveDirectory'
      login: username
      sid: userSid
      tenantId: tenant().tenantId
    }
  }
}

resource sqlDb 'Microsoft.Sql/servers/databases@2022-05-01-preview' = {
  parent: sqlServer
  name: toLower('sqldb-${webAppName}')
  location: 'eastus'
  sku: {
    name: 'GP_S_Gen5_1'
    tier: 'GeneralPurpose'
  }
  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    maxSizeBytes: 34359738368
    minCapacity: json('0.5')
  }
}

resource appConfigStore 'Microsoft.AppConfiguration/configurationStores@2023-03-01' = {
  name: toLower('appcs-${webAppName}')
  location: location
  sku: {
    name: 'free'
  }
}

resource configStoreKeyValue 'Microsoft.AppConfiguration/configurationStores/keyValues@2021-10-01-preview' = {
  parent: appConfigStore
  name: 'MyConfig'
  properties: {
    value: 'SomeGeneralConfig'
  }
}

module appServiceAppConfigurationRoleAssignment 'role-assignment.bicep' = {
  name: 'app-appcs-ra'
  params: {
    principalId: appService.identity.principalId
    roleDefinitionID: appConfigurationDataReaderRole
  }
}

module userAppConfigurationRoleAssignment 'role-assignment.bicep' = {
  name: 'usr-appcs-ra'
  params: {
    principalId: userSid
    roleDefinitionID: appConfigurationDataReaderRole
  }
}
