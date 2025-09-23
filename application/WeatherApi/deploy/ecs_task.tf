#https://registry.terraform.io/providers/hashicorp/aws/latest/docs/resources/ecs_task_definition
resource "aws_ecs_task_definition" "api_app" {
  family                   = "${var.name}-api"
  execution_role_arn       = aws_iam_role.ecs_task_execution_role.arn
  task_role_arn            = aws_iam_role.ecs_task_role.arn
  network_mode             = "awsvpc"
  requires_compatibilities = ["FARGATE"]
  cpu                      = "1024"
  memory                   = "3072"
  runtime_platform {
    operating_system_family = "LINUX"
    cpu_architecture        = "X86_64"
  }

  container_definitions = jsonencode([
    {
      name                   = "api"
      image                  = var.image_tag
      memory                 = 512
      essential              = true
      readonlyRootFilesystem = true
      portMappings = [
        {
          name          = "api"
          containerPort = 8080
          hostPort      = 8080
          protocol      = "tcp"
          appProtocol   = "http"
        }
      ]
      logConfiguration = {
        logDriver = "awslogs"
        options = {
          awslogs-group         = local.infra_output["aws_cloudwatch_log_group_name"]
          awslogs-region        = var.region
          awslogs-stream-prefix = "ecs"
        }
      }
      healthCheck = {
        command     = ["CMD-SHELL", "curl -f http://localhost:8080/healthcheck || exit 1"]
        interval    = 15
        retries     = 2
        timeout     = 10
        startPeriod = 60
      }
      environment = [
        {
          name  = "AWS_REGION",
          value = var.region
        },
        {
          name  = "OPENWEATHERMAP_BASE_URL",
          value = "http://api.openweathermap.org/data/2.5"
        }
      ]
      secrets = [
        {
          name      = "OpenWeatherMap__ApiKey"
          valueFrom = aws_secretsmanager_secret.openweathermap.arn
        }
      ]
    }
  ])
}