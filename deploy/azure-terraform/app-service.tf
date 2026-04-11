locals {
  app_service_name = "app-${var.name}"
}

resource "azurerm_service_plan" "default" {
  name                = "asp-${var.name}"
  location            = var.location
  resource_group_name = azurerm_resource_group.default.name

  os_type  = "Linux"
  sku_name = var.app_service_plan_sku_name
}

resource "random_bytes" "identity_sining_key" {
  length = 64
}

resource "azurerm_linux_web_app" "default" {
  name                = local.app_service_name
  location            = var.location
  resource_group_name = azurerm_resource_group.default.name

  service_plan_id           = azurerm_service_plan.default.id
  virtual_network_subnet_id = azurerm_subnet.app.id

  https_only = true

  identity {
    type = "SystemAssigned"
  }

  site_config {
    always_on = true

    application_stack {
      docker_registry_url = "https://${var.turnierplan_container_registry}"
      docker_image_name   = "${var.turnierplan_container_image}:${var.turnierplan_container_version}"
    }
  }

  app_settings = merge(var.turnierplan_additional_app_settings, {
    "Turnierplan__ApplicationUrl"      = var.app_service_custom_domain == null ? "https://${local.app_service_name}.azurewebsites.net" : "https://${var.app_service_custom_domain}"
    "Turnierplan__InitialUserName"     = var.turnierplan_initial_user
    "Turnierplan__InitialUserPassword" = var.turnierplan_initial_password

    "ApplicationInsights__ConnectionString" = azurerm_application_insights.default.connection_string
    "Database__ConnectionString"            = "Host=${azurerm_postgresql_flexible_server.default.fqdn};Database=${azurerm_postgresql_flexible_server_database.default.name};Username=${azurerm_postgresql_flexible_server.default.administrator_login};Password=${azurerm_postgresql_flexible_server.default.administrator_password}"
    "Identity__SigningKey"                  = random_bytes.identity_sining_key.base64

    "ImageStorage__Type"               = "Azure"
    "ImageStorage__StorageAccountName" = azurerm_storage_account.default.name
    "ImageStorage__ContainerName"      = azurerm_storage_container.images.name
  })
}

resource "azurerm_role_assignment" "application_blob_storage_contributor" {
  role_definition_name = "Storage Blob Data Contributor"
  principal_id         = azurerm_linux_web_app.default.identity[0].principal_id
  scope                = azurerm_storage_account.default.id
}

resource "azurerm_app_service_custom_hostname_binding" "default" {
  count = var.app_service_custom_domain == null ? 0 : 1

  resource_group_name = azurerm_resource_group.default.name
  app_service_name    = azurerm_linux_web_app.default.name
  hostname            = var.app_service_custom_domain

  lifecycle {
    ignore_changes = [ssl_state, thumbprint]
  }
}

resource "azurerm_app_service_managed_certificate" "default" {
  count = var.app_service_custom_domain == null ? 0 : 1

  custom_hostname_binding_id = azurerm_app_service_custom_hostname_binding.default[0].id
}

resource "azurerm_app_service_certificate_binding" "example" {
  count = var.app_service_custom_domain == null ? 0 : 1
  
  hostname_binding_id = azurerm_app_service_custom_hostname_binding.default[0].id
  certificate_id      = azurerm_app_service_managed_certificate.default[0].id
  ssl_state           = "SniEnabled"
}
