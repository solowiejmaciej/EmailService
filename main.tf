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

terraform {
  backend "azurerm"{
    resource_group_name = "Storage-TF.State-RG"
    storage_account_name = "storagestatetf"
    container_name = "tfdata"
    key = "terraform.tfstate"
  }
}

variable "imagebuild" {
  type = string
  description = "the latest image build version"
}

resource "azurerm_resource_group" "tf_rg_emailserviceapi" {
 name = "EmailService-RG"
 location = "westeurope" 
}

resource "azurerm_container_group" "tf_cg_emailserviceapi" {
  name = "EmailService-CG-V1"
  location = azurerm_resource_group.tf_rg_emailserviceapi.location
  resource_group_name = azurerm_resource_group.tf_rg_emailserviceapi.name

  ip_address_type = "public"
  dns_name_label = "emailserviceapi"
  os_type = "Linux"

  container {
    name = "emailserviceapi"
    image = "maciejsolowiej/emailserviceapi:${var.imagebuild}"
    cpu = "1"
    memory = "1"

    ports {
      port = 80
      protocol = "TCP"
    }
  }
}