terraform{
  required_providers{
  azurerm = {
      source = "hashicorp/azurerm",
      version = "2.46.0"
    }
  }
}

provider "azurerm" {
  features {
    
  }
}

resource "azurerm_resource_group" "tf_rg_emailserviceapi" {
 name = "EmailService-RG"
 location = "westeurope" 
}

resource "azurerm_container_group" "tf_cg_emailserviceapi" {
  name = "EmailService-CG"
  location = azurerm_resource_group.tf_rg_emailserviceapi.location
  resource_group_name = azurerm_resource_group.tf_rg_emailserviceapi.name

  ip_address_type = "public"
  dns_name_label = "emailserviceapi"
  os_type = "Linux"

  container {
    name = "emailserviceapi"
    image = "maciejsolowiej/emailserviceapi"
    cpu = "1"
    memory = "1"

    ports {
      port = 80
      protocol = "TCP"
    }
  }
}