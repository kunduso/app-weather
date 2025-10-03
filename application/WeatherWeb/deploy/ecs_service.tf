#https://registry.terraform.io/providers/hashicorp/aws/latest/docs/resources/ecs_service
resource "aws_ecs_service" "service" {
  name                              = "${var.name}-web"
  cluster                           = local.infra_output["aws_ecs_cluster_id"]
  task_definition                   = aws_ecs_task_definition.web_app.arn
  desired_count                     = 2
  force_new_deployment              = true
  health_check_grace_period_seconds = 120
  load_balancer {
    target_group_arn = local.infra_output["aws_lb_target_group_arn"]
    container_name   = "web"
    container_port   = "8081"
  }
  launch_type = "FARGATE"
  network_configuration {
    security_groups  = [local.infra_output["container_security_group_id"]]
    subnets          = local.infra_output["subnet_ids"]
    assign_public_ip = false
  }
  service_connect_configuration {
    enabled   = true
    namespace = local.infra_output["service_namespace_arn"]
    service {
      port_name      = "http"
      discovery_name = "${var.name}-web"
      client_alias {
        port     = 8081
        dns_name = "${var.name}-web"
      }
    }
    service {
      port_name = "api"
      client_alias {
        port     = 8080
        dns_name = "${var.name}-api"
      }
    }
    log_configuration {
      log_driver = "awslogs"
      options = {
        awslogs-group         = local.infra_output["service_connect_log_group_name"]
        awslogs-region        = var.region
        awslogs-stream-prefix = "service-connect"
      }
    }
  }
}