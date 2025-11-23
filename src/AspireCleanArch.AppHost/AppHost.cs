var builder = DistributedApplication.CreateBuilder(args);

// SQL Server - Database for persistent storage
var sqlServer = builder.AddSqlServer("sqlserver")
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("aspire-cleanarch-db");

// Redis - Distributed caching
var redis = builder.AddRedis("redis")
    .WithLifetime(ContainerLifetime.Persistent);

// RabbitMQ - Message broker for events
var rabbitMq = builder.AddRabbitMQ("rabbitmq")
    .WithLifetime(ContainerLifetime.Persistent);


// gRPC Server - Internal service-to-service communication (Query operations)
var grpcService = builder.AddProject<Projects.AspireCleanArch_gRPC>("grpc-service")
    .WithReference(sqlServer)
    .WithReference(redis);

// REST API - Public-facing API (CRUD operations)
builder.AddProject<Projects.AspireCleanArch_Api>("api-service")
   .WithReference(sqlServer)
   .WithReference(redis)
   .WithOtlpExporter()
  .WithHttpHealthCheck("/health")
   .WithReference(rabbitMq)
   .WithReference(grpcService)
   .WithExternalHttpEndpoints();



builder.Build().Run();
