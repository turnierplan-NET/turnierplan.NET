variable "name" {
  description = "Name suffix for all resources"
  type        = string
  nullable    = false
}

variable "location" {
  description = "Location of the deployed resources"
  type        = string
  nullable    = false
}

variable "turnierplan_container_registry" {
  description = "The container registry to pull the image from"
  type        = string
  default     = "ghcr.io"
  nullable    = false
}

variable "turnierplan_container_image" {
  description = "The name of the container image to pull"
  type        = string
  default     = "turnierplan-net/turnierplan"
  nullable    = false
}

variable "turnierplan_container_version" {
  description = "The name and tag of the container image to pull"
  type        = string
  default     = "2026.2.0"
  nullable    = false
}

variable "turnierplan_initial_user" {
  description = "The user name for the initially created admin user"
  type        = string
  nullable    = false
}

variable "turnierplan_initial_password" {
  description = "The password to initially set for the created admin user"
  type        = string
  nullable    = false
}

variable "turnierplan_additional_app_settings" {
  description = "Additional configuration values for turnierplan.NET"
  type        = map(string)
  nullable    = false
}

variable "app_service_plan_sku_name" {
  description = "The SKU name to use for the app service plan"
  type        = string
  nullable    = false
}

variable "app_service_custom_domain" {
  description = "The domain name which should be bound to the app service (e.g. 'turnierplan.example.com') or null if no custom domain should be used."
  type        = string
  nullable    = true
  default     = null
}

variable "app_insights_retention_in_days" {
  description = "The retention period for application insights logs"
  type        = number
  nullable    = false
}

variable "storage_account_replication_type" {
  description = "The replication type to use for the storage account"
  type        = string
  nullable    = false
}

variable "postgresql_availability_zone" {
  description = "The availability zone to deploy the PostgreSQL server in"
  type        = number
  nullable    = false
}

variable "postgresql_sku_name" {
  description = "The name of the SKU to use for the PostgreSQL server"
  type        = string
  nullable    = false
}

variable "postgresql_storage_size_mb" {
  description = "The storage size in MB for the PostgreSQL server"
  type        = number
  nullable    = false
}

variable "postgresql_storage_tier" {
  description = "The storage tier for the PostgreSQL server"
  type        = string
  nullable    = false
}

variable "postgresql_charset" {
  description = "The charset to use for the PostgreSQL database"
  type        = string
  nullable    = false
}

variable "postgresql_collation" {
  description = "The collation to use for the PostgreSQL database"
  type        = string
  nullable    = false
}
