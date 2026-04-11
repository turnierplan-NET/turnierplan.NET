resource "random_password" "psql_admin" {
  length  = 32
  special = false
}

resource "azurerm_postgresql_flexible_server" "default" {
  name                = "psql-${var.name}"
  location            = var.location
  resource_group_name = azurerm_resource_group.default.name

  public_network_access_enabled = false
  delegated_subnet_id           = azurerm_subnet.database.id
  private_dns_zone_id           = azurerm_private_dns_zone.database.id

  administrator_login    = "tpsqladm"
  administrator_password = random_password.psql_admin.result

  authentication {
    active_directory_auth_enabled = false
    password_auth_enabled         = true
  }

  version      = "18"
  sku_name     = var.postgresql_sku_name
  zone         = var.postgresql_availability_zone
  storage_mb   = var.postgresql_storage_size_mb
  storage_tier = var.postgresql_storage_tier
}

resource "azurerm_postgresql_flexible_server_database" "default" {
  name      = "turnierplan"
  server_id = azurerm_postgresql_flexible_server.default.id
  charset   = var.postgresql_charset
  collation = var.postgresql_collation

  lifecycle {
    prevent_destroy = true
  }
}
