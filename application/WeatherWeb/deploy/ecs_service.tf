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
  service_registries {
    registry_arn = aws_service_discovery_service.web.arn
  }
}