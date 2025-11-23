var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.AspireCleanArch_gRPC>("aspirecleanarch-grpc");

builder.Build().Run();
