using './main.bicep'

param webAppName = 'man-iden'
param sku = 'B1'
param linuxFxVersion = 'DOTNETCORE|8.0'
param sqlUserId = 'mysqluser'
param sqlPassword = 'Mysqlpassword!'

param keyVaultMySecretValue = ''

param username = ''
param userSid = ''
