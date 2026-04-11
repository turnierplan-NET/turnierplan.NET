resource "azurerm_storage_account" "default" {
  name                = "st${replace(var.name, "-", "")}"
  location            = var.location
  resource_group_name = azurerm_resource_group.default.name

  access_tier              = "Hot"
  account_tier             = "Standard"
  account_replication_type = var.storage_account_replication_type

  public_network_access_enabled = true

  lifecycle {
    prevent_destroy = true
  }
}

resource "azurerm_storage_container" "images" {
  name                  = "${var.name}-images"
  storage_account_id    = azurerm_storage_account.default.id
  container_access_type = "blob"
}
