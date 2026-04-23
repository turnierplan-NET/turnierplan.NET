resource "azurerm_resource_group" "default" {
  name     = "rg-${var.name}"
  location = var.location
}
