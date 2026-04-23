resource "azurerm_log_analytics_workspace" "default" {
  name                = "log-${var.name}"
  location            = var.location
  resource_group_name = azurerm_resource_group.default.name
  sku                 = "PerGB2018"
}

resource "azurerm_application_insights" "default" {
  name                = "appi-${var.name}"
  location            = var.location
  resource_group_name = azurerm_resource_group.default.name
  workspace_id        = azurerm_log_analytics_workspace.default.id
  application_type    = "web"
  retention_in_days   = var.app_insights_retention_in_days
}
