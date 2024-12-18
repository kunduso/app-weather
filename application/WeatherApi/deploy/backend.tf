terraform {
  backend "s3" {
    bucket  = "kunduso-terraform-remote-bucket"
    encrypt = true
    key     = "tf/app-weather/deploy-api/terraform.tfstate"
    region  = "us-east-2"
  }
}