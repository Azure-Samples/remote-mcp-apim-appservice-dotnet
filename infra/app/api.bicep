param name string
param location string = resourceGroup().location
param tags object = {}
param appServicePlanId string
param serviceName string = 'api'

resource webApp 'Microsoft.Web/sites@2023-12-01' = {
  name: name
  location: location
  tags: union(tags, { 'azd-service-name': serviceName })
  kind: 'app'
  properties: {
    serverFarmId: appServicePlanId
    siteConfig: {
      appSettings: [
        // Add any required ASP.NET Core app settings here
      ]
      linuxFxVersion: 'DOTNETCORE|8.0'
    }
  }
}

output SERVICE_API_NAME string = webApp.name
