using RemoteMcpMsGraph.Tools;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMcpServer()
    .WithHttpTransport(options =>
    {
        options.Stateless = true;
    })
    .WithTools<ShowUserProfileTool>();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.MapMcp();

app.Run();
