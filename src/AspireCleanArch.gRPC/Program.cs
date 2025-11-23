using AspireCleanArch.gRPC.Services;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddGrpc();

// TODO: Add gRPC service implementations
// TODO: Add Application Services (MediatR, Repositories, etc.)
// TODO: Add Infrastructure Services (DbContext, Redis)

var app = builder.Build();



// Map gRPC services
app.MapGrpcService<GreeterService>();

// TODO: Map additional gRPC services
// app.MapGrpcService<OrdersApiImplementation>();
// app.MapGrpcService<ProductsApiImplementation>();
// app.MapGrpcService<VendorsApiImplementation>();
// app.MapGrpcService<PaymentsApiImplementation>();
// app.MapGrpcService<UsersApiImplementation>();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
