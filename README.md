[![License: Unlicense](https://img.shields.io/badge/license-Unlicense-white.svg)](https://choosealicense.com/licenses/unlicense/) [![GitHub pull-requests closed](https://img.shields.io/github/issues-pr-closed/kunduso/app-weather)](https://github.com/kunduso/app-weather/pulls?q=is%3Apr+is%3Aclosed) [![GitHub pull-requests](https://img.shields.io/github/issues-pr/kunduso/app-weather)](https://GitHub.com/kunduso/app-weather/pull/) [![GitHub issues-closed](https://img.shields.io/github/issues-closed/kunduso/app-weather)](https://github.com/kunduso/app-weather/issues?q=is%3Aissue+is%3Aclosed) [![GitHub issues](https://img.shields.io/github/issues/kunduso/app-weather)](https://GitHub.com/kunduso/app-weather/issues/) [![terraform-infra-provisioning](https://github.com/kunduso/app-weather/actions/workflows/terraform.yml/badge.svg?branch=main)](https://github.com/kunduso/app-weather/actions/workflows/terraform.yml) [![checkov-scan](https://github.com/kunduso/app-weather/actions/workflows/code-scan.yml/badge.svg?branch=main)](https://github.com/kunduso/app-weather/actions/workflows/code-scan.yml) [![CodeQL](https://github.com/kunduso/app-weather/actions/workflows/github-code-scanning/codeql/badge.svg?branch=main)](https://github.com/kunduso/app-weather/actions/workflows/github-code-scanning/codeql) [![weather-web-build-deploy](https://github.com/kunduso/app-weather/actions/workflows/app-web-ci-cd.yml/badge.svg?branch=main)](https://github.com/kunduso/app-weather/actions/workflows/app-web-ci-cd.yml) [![weather-api-build-deploy](https://github.com/kunduso/app-weather/actions/workflows/app-api-ci-cd.yml/badge.svg?branch=main)](https://github.com/kunduso/app-weather/actions/workflows/app-api-ci-cd.yml)


# Weather Application

A containerized weather application built with .NET 8, consisting of a web frontend and an API backend. The application fetches weather data from OpenWeatherMap API and is deployed to AWS ECS through automated GitHub Actions pipelines.

## Architecture

The application consists of two main components:
- **Weather API**: A .NET Web API that interfaces with OpenWeatherMap
- **Weather Web**: A .NET Web Application that serves as the frontend

Both components are containerized and run in AWS ECS Fargate with proper security considerations.

## Infrastructure and Deployment Stages

The deployment architecture is split into two main stages:

### Stage 1: Foundation Infrastructure (Terraform)
Located in the `/infrastructure` folder, this stage creates:
- VPC with public and private subnets
- Internet Gateway and NAT Gateway
- Route tables and associated routes
- Application Load Balancers (ALB)
- Target Groups
- Security Groups
- ECS Cluster
- ECR Repositories

### Stage 2: Application Deployment (GitHub Actions)
After container images are built and pushed to ECR, this stage creates:
- ECS Task Definitions
- ECS Services
- IAM Task Execution Roles
- IAM Task Roles
- Service Discovery configuration
- Container-specific security groups

## Deployment Pipeline

The application uses GitHub Actions for automated deployment:

### CI/CD Workflow
1. Code changes trigger the pipeline
2. Security scans run (Checkov and CodeQL)
3. Docker images are built and pushed to ECR
4. ECS resources are created/updated:
   - Task definitions are registered
   - Task and execution roles are created/updated
   - ECS services are deployed/updated

### Pipeline Files
- `.github/workflows/app-api-ci-cd.yml`: API deployment pipeline
- `.github/workflows/app-web-ci-cd.yml`: Web application deployment pipeline
- `.github/workflows/terraform.yml`: Infrastructure provisioning pipeline
- `.github/workflows/code-scan.yml`: Security scanning pipeline

## Prerequisites

- Docker
- .NET 8 SDK
- AWS CLI (for local testing with AWS resources)
- OpenWeatherMap API key

## Local Development


## Infrastructure Management

### Prerequisites
- Terraform installed locally
- AWS credentials configured
- Appropriate IAM permissions

### Foundation Infrastructure Deployment
1. Navigate to the infrastructure directory:

Apply changes (normally handled by GitHub Actions):


Note: Infrastructure changes should be made through pull requests to trigger the automated pipeline.

Application Deployment
Container Build and Push
GitHub Actions automatically builds containers on push

Images are tagged and pushed to ECR

SHA and version tags are applied

ECS Deployment
GitHub Actions automatically:

Creates/updates ECS task definitions

Configures IAM roles and policies

Creates/updates ECS services

Manages service discovery entries

Environment Variables and Secrets
Required Secrets
OpenWeatherMap__ApiKey: Stored in AWS Secrets Manager

AWS_ACCESS_KEY_ID: GitHub Actions secret for AWS access

AWS_SECRET_ACCESS_KEY: GitHub Actions secret for AWS access

AWS_REGION: GitHub Actions secret for AWS region

Environment Configuration
Development: Uses local environment variables

Production: Uses AWS Secrets Manager

Monitoring and Logging
Container logs available in CloudWatch Logs

ALB access logs stored in S3

Container health checks configured in task definitions

CloudWatch metrics for ECS services and ALB

Security Features
Secure secret management using AWS Secrets Manager

Non-root container execution

Automated security scanning

HTTPS enforcement

Regular dependency updates

Container health monitoring

IAM role-based access control

Network security groups

Private subnets for containers

CI/CD Pipeline Details
Build and Deploy Process
Code changes trigger GitHub Actions workflow

Security scans run in parallel

Docker images are built and tagged

Images are pushed to Amazon ECR

ECS task definitions are updated

IAM roles and policies are configured

ECS services are created/updated

Automated Testing
Security scanning with Checkov

Code analysis with CodeQL

Container vulnerability scanning

Infrastructure validation

Contributing
Fork the repository

Create a feature branch

Commit your changes

Push to the branch

Create a Pull Request

## License
This project is licensed under the Unlicense - see the LICENSE file for details.

## Acknowledgments

- OpenWeatherMap API for weather data
- AWS for cloud infrastructure
- GitHub Actions for CI/CD
- Terraform for infrastructure management
- Amazon Q for AI-assisted development and documentation support