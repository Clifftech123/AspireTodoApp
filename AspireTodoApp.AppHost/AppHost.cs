using CommunityToolkit.Aspire.Hosting.MailPit;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithPgAdmin()
    .AddDatabase("tododb");

var redis = builder.AddRedis("cache");

var mail = builder.AddMailPit("mail");

var server = builder.AddProject<Projects.AspireTodoApp_Server>("server")
    .WithReference(postgres)
    .WithReference(redis)
    .WithReference(mail)
    .WaitFor(postgres)
    .WaitFor(redis)
    .WithHttpHealthCheck("/health")
    .WithExternalHttpEndpoints();

var webfrontend = builder.AddViteApp("webfrontend", "../frontend")
    .WithReference(server)
    .WaitFor(server);

server.PublishWithContainerFiles(webfrontend, "wwwroot");

builder.Build().Run();
 
 