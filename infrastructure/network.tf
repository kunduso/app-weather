#https://registry.terraform.io/providers/hashicorp/aws/latest/docs/resources/service_discovery_http_namespace
resource "aws_service_discovery_http_namespace" "namespace" {
  name        = var.name
  description = "The namespace for the ECS cluster. "
}
resource "aws_vpc" "this" {
  #checkov:skip=CKV2_AWS_11: This is non prod and hence disabled.
  cidr_block           = var.vpc_cidr
  enable_dns_hostnames = true
  enable_dns_support   = true
  tags = {
    "Name" = "${var.name}"
  }
}
resource "aws_subnet" "public" {
  count             = length(var.subnet_cidr_public)
  vpc_id            = aws_vpc.this.id
  cidr_block        = var.subnet_cidr_public[count.index]
  availability_zone = data.aws_availability_zones.available.names[count.index]
  tags = {
    "Name" = "${var.name}-public-${count.index + 1}"
  }
}
resource "aws_route_table" "this_rt" {
  vpc_id = aws_vpc.this.id
  tags = {
    "Name" = "${var.name}-route-table"
  }
}
resource "aws_route_table_association" "public" {
  count          = length(var.subnet_cidr_public)
  subnet_id      = element(aws_subnet.public.*.id, count.index)
  route_table_id = aws_route_table.this_rt.id
}
resource "aws_internet_gateway" "this_igw" {
  vpc_id = aws_vpc.this.id
  tags = {
    "Name" = "${var.name}-gateway"
  }
}
resource "aws_route" "internet_route" {
  destination_cidr_block = "0.0.0.0/0"
  route_table_id         = aws_route_table.this_rt.id
  gateway_id             = aws_internet_gateway.this_igw.id
}
resource "aws_subnet" "private" {
  count             = length(var.subnet_cidr_public)
  vpc_id            = aws_vpc.this.id
  cidr_block        = var.subnet_cidr_private[count.index]
  availability_zone = data.aws_availability_zones.available.names[count.index]
  tags = {
    "Name" = "${var.name}-private-${count.index + 1}"
  }
}
# Create 2 Elastic IPs for NAT Gateways
resource "aws_eip" "nat" {
  count  = length(var.subnet_cidr_public)
  domain = "vpc"
  tags = {
    Name = "${var.name}-nat-eip-${count.index + 1}"
  }

  depends_on = [aws_internet_gateway.this_igw]
}

# Create 2 NAT Gateways, one in each public subnet
resource "aws_nat_gateway" "this" {
  count         = 2
  allocation_id = aws_eip.nat[count.index].id
  subnet_id     = aws_subnet.public[count.index].id

  tags = {
    Name = "${var.name}-nat-gateway-${count.index + 1}"
  }

  depends_on = [aws_internet_gateway.this_igw]
}

# Create separate route tables for each private subnet
resource "aws_route_table" "private" {
  count  = length(var.subnet_cidr_private)
  vpc_id = aws_vpc.this.id

  tags = {
    Name = "${var.name}-private-rt-${count.index + 1}"
  }
}

# Add routes to NAT Gateways in the private route tables
resource "aws_route" "private_nat_gateway" {
  count                  = length(var.subnet_cidr_private)
  route_table_id         = aws_route_table.private[count.index].id
  destination_cidr_block = "0.0.0.0/0"
  nat_gateway_id         = aws_nat_gateway.this[count.index].id
}

# Associate private subnets with their respective route tables
resource "aws_route_table_association" "private" {
  count          = length(var.subnet_cidr_private)
  subnet_id      = aws_subnet.private[count.index].id
  route_table_id = aws_route_table.private[count.index].id
}