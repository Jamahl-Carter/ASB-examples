terraform {
  required_providers {
    azurerm = {
      source = "hashicorp/azurerm"
      version = ">= 2.26"
    }
  }
}

provider "azurerm" {
  features {}
}

resource "random_string" "random" {
  length = 5
  special = false
}

resource "azurerm_resource_group" "rg" {
  name     = "rg-sync-adapter-example"
  location = "australiaeast"
}

resource "azurerm_servicebus_namespace" "ns" {
  name                = "ns-servicebus-${random_string.random.result}"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  sku                 = "Standard"
}

resource "azurerm_servicebus_queue" "request" {
  name                = "RequestQueue"
  resource_group_name = azurerm_resource_group.rg.name
  namespace_name      = azurerm_servicebus_namespace.ns.name
  max_delivery_count  = 10
}

resource "azurerm_servicebus_queue" "reply" {
  name                = "ReplyQueue"
  resource_group_name = azurerm_resource_group.rg.name
  namespace_name      = azurerm_servicebus_namespace.ns.name
  requires_session    = true
  max_delivery_count  = 10
}