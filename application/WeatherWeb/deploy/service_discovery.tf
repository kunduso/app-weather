# Route 53 Service Discovery for Web Service
resource "aws_service_discovery_service" "web" {
  name = "${var.name}-web"

  dns_config {
    namespace_id = local.infra_output["service_discovery_namespace_id"]
    
    dns_records {
      ttl  = 10
      type = "A"
    }
    
    routing_policy = "MULTIVALUE"
  }
}