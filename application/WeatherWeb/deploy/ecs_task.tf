#https://registry.terraform.io/providers/hashicorp/aws/latest/docs/resources/ecs_task_definition
resource "aws_ecs_task_definition" "web_app" {
  family                   = var.name
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
      name                   = "web"
      image                  = var.image_tag
      memory                 = 512
      essential              = true
      readonlyRootFilesystem = true
      portMappings = [
        {
          name          = "http"
          containerPort = 8081
          hostPort      = 8081
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
        command     = ["CMD-SHELL", "curl -f http://localhost:8081/healthcheck>> /proc/1/fd/1 2>&1 || exit 1"]
        interval    = 30
        retries     = 3
        timeout     = 5
        startPeriod = 10
      }
      environment = [
        {
          name  = "AWS_REGION",
          value = var.region
        },
        {
          name  = "WEATHER_API_BASE_URL",
          value = "http://app-weather-api:8080"
        }
      ]
    }
  ])
}