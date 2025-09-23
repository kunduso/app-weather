#https://registry.terraform.io/providers/hashicorp/aws/latest/docs/resources/ecs_service
resource "aws_ecs_service" "service" {
  name                              = "${var.name}-api"
  cluster                           = local.infra_output["aws_ecs_cluster_id"]
  task_definition                   = aws_ecs_task_definition.api_app.arn
  desired_count                     = 1
  force_new_deployment              = true
  launch_type                       = "FARGATE"
  health_check_grace_period_seconds = 120
  network_configuration {
    security_groups  = [local.infra_output["container_security_group_id"]]
    subnets          = local.infra_output["subnet_ids"]
    assign_public_ip = false
  }
  service_connect_configuration {
    enabled   = true
    namespace = local.infra_output["service_namespace_arn"]
    service {
      port_name      = "api"
      discovery_name = "${var.name}-api"
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
resource "aws_secretsmanager_secret" "openweathermap" {
  #checkov:skip=CKV2_AWS_57: This secret is managed in the Open Weather Map org.
  name                    = "${var.name}-openweathermap-api-key"
  recovery_window_in_days = 0
  kms_key_id              = local.infra_output["kms_arn"]
}

resource "aws_secretsmanager_secret_version" "openweathermap" {
  secret_id = aws_secretsmanager_secret.openweathermap.id
  secret_string = jsonencode({
    "OpenWeatherMap__ApiKey" = var.openweathermap_api_key
  })
}