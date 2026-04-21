resource "azurerm_virtual_network" "default" {
  name                = "vnet-${var.name}"
  location            = var.location
  resource_group_name = azurerm_resource_group.default.name
  address_space       = ["10.0.0.0/16"]
}

resource "azurerm_subnet" "app" {
  name                 = "subnet-app-${var.name}"
  resource_group_name  = azurerm_resource_group.default.name
  virtual_network_name = azurerm_virtual_network.default.name
  address_prefixes     = ["10.0.1.0/24"]

  delegation {
    name = "app"
    service_delegation {
      name    = "Microsoft.Web/serverFarms"
      actions = ["Microsoft.Network/virtualNetworks/subnets/action"]
    }
  }
}

resource "azurerm_subnet" "database" {
  name                 = "subnet-database-${var.name}"
  resource_group_name  = azurerm_resource_group.default.name
  virtual_network_name = azurerm_virtual_network.default.name
  address_prefixes     = ["10.0.2.0/24"]
  service_endpoints    = ["Microsoft.Storage"]

  delegation {
    name = "psql"
    service_delegation {
      name    = "Microsoft.DBforPostgreSQL/flexibleServers"
      actions = ["Microsoft.Network/virtualNetworks/subnets/join/action"]
    }
  }
}

resource "azurerm_private_dns_zone" "database" {
  name                = "dnszone-${var.name}.postgres.database.azure.com"
  resource_group_name = azurerm_resource_group.default.name
}

resource "azurerm_private_dns_zone_virtual_network_link" "database" {
  name                  = "database-vnet-link"
  private_dns_zone_name = azurerm_private_dns_zone.database.name
  virtual_network_id    = azurerm_virtual_network.default.id
  resource_group_name   = azurerm_resource_group.default.name
  depends_on            = [azurerm_subnet.database]
}
