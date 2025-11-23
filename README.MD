

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Aspire](https://img.shields.io/badge/Aspire-9.0-512BD4?logo=dotnet)](https://learn.microsoft.com/en-us/dotnet/aspire/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![Build](https://img.shields.io/badge/build-passing-brightgreen.svg)]()

> **Enterprise-grade e-commerce platform** built with modern .NET technologies, Clean Architecture, CQRS, Domain-Driven Design, gRPC, and Event-Driven Architecture.

![Architecture Diagram](docs/architecture-diagram.png)

---

## ✨ Features

### 🏪 **Multi-Vendor Marketplace**
- Vendor registration and approval workflow
- Product catalog management
- Real-time inventory tracking
- Commission-based revenue model
- Vendor analytics dashboard

### 🛒 **Order Management**
- Shopping cart with real-time stock validation
- Multi-vendor order support
- Order lifecycle tracking (Pending → Delivered)
- Cancellation and refund workflows
- Order history and tracking

### 💳 **Payment Processing**
- Multiple payment methods (Credit Card, PayPal, Wallet)
- Stripe and PayPal integration
- Secure payment processing
- Automated refund handling
- Payment history and receipts

### 👤 **User Management**
- ASP.NET Core Identity integration
- JWT-based authentication
- Role-based authorization (Customer, Vendor, Admin)
- User profiles and address management
- Email verification

### 📊 **Admin Dashboard**
- System-wide metrics and analytics
- Vendor approval and management
- Order oversight
- Revenue tracking
- Real-time monitoring

---

## 🏗️ Architecture


### **Communication Patterns**

- **REST API** - External client communication
- **gRPC** - Internal service-to-service synchronous calls
- **RabbitMQ** - Asynchronous event-driven messaging
- **Redis** - Distributed caching

---

## 🛠️ Technology Stack

### **Backend**
- **.NET 9** - Latest .NET framework
- **ASP.NET Core** - Web API framework
- **Entity Framework Core 9** - ORM
- **MediatR** - CQRS implementation
- **FluentValidation** - Input validation
- **AutoMapper** - Object mapping

### **Messaging & Communication**
- **RabbitMQ** - Message broker
- **MassTransit** - Distributed application framework
- **gRPC** - High-performance RPC framework
- **SignalR** - Real-time communication (optional)

### **Data & Caching**
- **SQL Server** - Primary database
- **Redis** - Distributed cache
- **Dapper** - Lightweight ORM for queries (optional)

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

## 🚀 Getting Started

### **Prerequisites**

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)
- [SQL Server](https://www.microsoft.com/sql-server) (or use Docker)
- [RabbitMQ](https://www.rabbitmq.com/) (or use Docker)

### **Installation**

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/ecommerce-platform.git
   cd ecommerce-platform
   ```

2. **Restore packages**
   ```bash
   dotnet restore
   ```

3. **Update connection strings**
   
   Edit `src/AspireCleanArch.Api/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=ECommerceDb;Trusted_Connection=True;"
     }
   }
   ```

4. **Run database migrations**
   ```bash
   cd src/AspireCleanArch.Infrastructure
   dotnet ef database update --startup-project ../AspireCleanArch.Api
   ```

5. **Run with Aspire** (Recommended)
   ```bash
   cd src/AspireCleanArch.AppHost
   dotnet run
   ```

   Or run the API directly:
   ```bash
   cd src/AspireCleanArch.Api
   dotnet run
   ```

6. **Access the application**
   - API: https://localhost:7001
   - Swagger: https://localhost:7001/swagger
   - Aspire Dashboard: http://localhost:15888

### **Using Docker Compose** (Alternative)

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



---

## 🔑 Key Design Patterns

- **Clean Architecture** - Clear separation of concerns
- **CQRS** - Command Query Responsibility Segregation
- **Domain-Driven Design** - Rich domain models
- **Repository Pattern** - Data access abstraction
- **Unit of Work** - Transaction management
- **Result Pattern** - Functional error handling
- **Outbox Pattern** - Reliable event publishing
- **Gateway Pattern** - External service abstraction

---

## 📊 Domain Model

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

## 🔄 Event Flow

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





## 🐳 Docker Support

### **Build Docker Image**

```bash
docker build -t ecommerce-api:latest -f src/AspireCleanArch.Api/Dockerfile .
```

### **Run with Docker Compose**

```bash
docker-compose up -d
```

---

## 📦 NuGet Packages

All package versions are managed centrally in `Directory.Packages.props`:

- **Aspire** - Cloud-native orchestration
- **Entity Framework Core** - ORM
- **MediatR** - CQRS
- **FluentValidation** - Validation
- **MassTransit** - Messaging
- **Grpc.AspNetCore** - gRPC
- **Serilog** - Logging
- **xUnit** - Testing

[View complete package list](Directory.Packages.props)

---

## 🤝 Contributing

Contributions are welcome! Please read our [Contributing Guidelines](CONTRIBUTING.md) first.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

---

## 🎨 OPTIONAL: Add Badges

Add these at the top of your README for a professional look:


[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-12.0-239120?logo=c-sharp)](https://learn.microsoft.com/dotnet/csharp/)
[![Aspire](https://img.shields.io/badge/Aspire-9.0-512BD4?logo=dotnet)](https://learn.microsoft.com/dotnet/aspire/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg)](http://makeapullrequest.com)
[![GitHub issues](https://img.shields.io/github/issues/yourusername/ecommerce-platform)](https://github.com/yourusername/ecommerce-platform/issues)
[![GitHub stars](https://img.shields.io/github/stars/yourusername/ecommerce-platform)](https://github.com/yourusername/ecommerce-platform/stargazers)
