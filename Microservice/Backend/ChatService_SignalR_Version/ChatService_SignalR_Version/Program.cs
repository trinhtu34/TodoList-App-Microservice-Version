using Cassandra;

var cluster = Cluster.Builder()
    .AddContactPoint("localhost") // Replace with your ScyllaDB IP
    .WithPort(9042) // Default ScyllaDB port
    .Build();

Cassandra.ISession session = await cluster.ConnectAsync("chat_service");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
