

[![.NET](https://img.shields.io/badge/.NET-1.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Aspire](https://img.shields.io/badge/Aspire-1.0-512BD4?logo=dotnet)](https://learn.microsoft.com/en-us/dotnet/aspire/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![Build](https://img.shields.io/badge/build-passing-brightgreen.svg)]()

> **Enterprise-grade e-commerce platform** built with modern .NET technologies, Clean Architecture, CQRS, Domain-Driven Design, gRPC, and Event-Driven Architecture.

![Architecture Diagram](docs/architecture-diagram.png)

---

## ✨ Features

###  **Multi-Vendor Marketplace**
- Vendor registration and approval workflow
- Product catalog management
- Real-time inventory tracking
- Commission-based revenue model
- Vendor analytics dashboard

###  **Order Management**
- Shopping cart with real-time stock validation
- Multi-vendor order support
- Order lifecycle tracking (Pending → Delivered)
- Cancellation and refund workflows
- Order history and tracking

###  **Payment Processing**
- Multiple payment methods (Credit Card, PayPal, Wallet)
- Stripe and PayPal integration
- Secure payment processing
- Automated refund handling
- Payment history and receipts

###  **User Management**
- ASP.NET Core Identity integration
- JWT-based authentication
- Role-based authorization (Customer, Vendor, Admin)
- User profiles and address management
- Email verification

###  **Admin Dashboard**
- System-wide metrics and analytics
- Vendor approval and management
- Order oversight
- Revenue tracking
- Real-time monitoring

---

##  Architecture


### **Communication Patterns**

- **REST API** - External client communication
- **gRPC** - Internal service-to-service synchronous calls
- **RabbitMQ** - Asynchronous event-driven messaging
- **Redis** - Distributed caching

---

##  Technology Stack

### **Backend**
- **.NET 10** - Latest .NET framework
- **ASP.NET Core** - Web API framework
- **Entity Framework Core 10** - ORM
- **MediatR** - CQRS implementation
- **FluentValidation** - Input validation
- **AutoMapper** - Object mapping

### **Messaging & Communication**
- **RabbitMQ** - Message broker
- **MassTransit** - Distributed application framework
- **gRPC** - High-performance RPC framework
- **SignalR** - Real-time communication 

### **Data & Caching**
- **SQL Server** - Primary database
- **Redis** - Distributed cache


### **Authentication & Security**
- **ASP.NET Core Identity** - User management
- **JWT Bearer** - Token-based authentication
- **BCrypt** - Password hashing

### **External Integrations**
- **Stripe** - Payment processing
- **PayPal** - Alternative payment method
- **SendGrid** - Email notifications

### **DevOps & Monitoring**
- **.NET Aspire** - Cloud-native orchestration
- **Docker** - Containerization
- **OpenTelemetry** - Distributed tracing
- **Serilog** - Structured logging
- **Health Checks** - Service health monitoring

### **Testing**
- **xUnit** - Unit testing framework
- **FluentAssertions** - Assertion library
- **Moq/NSubstitute** - Mocking framework
- **Testcontainers** - Integration testing

---

## 📁 Project Structure

```
AspireCleanArch.NetI0/
├── src/
│   ├── AspireCleanArch.Shared/              # Shared DTOs, Events, Protos
│   ├── AspireCleanArch.Domain/              # Domain entities & business logic
│   ├── AspireCleanArch.Application/         # CQRS, Use cases
│   ├── AspireCleanArch.Infrastructure/      # EF Core, RabbitMQ, gRPC clients
│   ├── AspireCleanArch.Api/                 # REST API & gRPC servers
│   └── AspireCleanArch.AppHost/             # Aspire orchestration
│
├── tests/
│   ├── Domain.Tests/                        # Domain unit tests
│   ├── Application.Tests/                   # Application tests
│   └── Api.Tests/                           # Integration tests
│
├── docs/                                     # Documentation
├── Directory.Packages.props                  # Centralized package management
└── Directory.Build.props                     # Common build properties
```

---

##  Getting Started

### **Prerequisites**

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)
- [SQL Server](https://www.microsoft.com/sql-server) (or use Docker)
- [RabbitMQ](https://www.rabbitmq.com/) (or use Docker)

### **Installation**

1. **Clone the repository**
   ```bash
   git clone https://github.com/Clifftech123/AspireCleanArch.Net10.git
   
   ```


### **Using Docker Compose** 

```bash
docker-compose up -d
```

---

## 🧪 Running Tests

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test project
dotnet test tests/AspireCleanArch.Domain.Tests
```



##  Key Design Patterns

- **Clean Architecture** - Clear separation of concerns
- **CQRS** - Command Query Responsibility Segregation
- **Domain-Driven Design** - Rich domain models
- **Repository Pattern** - Data access abstraction
- **Unit of Work** - Transaction management
- **Result Pattern** - Functional error handling
- **Outbox Pattern** - Reliable event publishing
- **Gateway Pattern** - External service abstraction

---

##  Domain Model

### **Core Entities**

- **Vendor** - Marketplace sellers
- **Product** - Product catalog
- **Order** - Customer orders
- **OrderItem** - Order line items
- **Payment** - Payment transactions
- **User** - System users

### **Entity Relationships**

```
Vendor (1) ──► (N) Product
User (1) ──► (N) Order
Order (1) ──► (N) OrderItem
Order (1) ──► (1) Payment
OrderItem (N) ──► (1) Product
OrderItem (N) ──► (1) Vendor
```

---

##  Event Flow

### **Order Creation Flow**

```
1. User creates order (REST API)
2. Order Service validates products (gRPC → Vendor Service)
3. Order Service reserves stock (gRPC → Vendor Service)
4. Order Service creates order (saves to database)
5. Order Service publishes OrderPlacedEvent (RabbitMQ)
6. Payment Service consumes event → processes payment
7. Payment Service publishes PaymentSucceededEvent (RabbitMQ)
8. Order Service consumes event → confirms order
```





