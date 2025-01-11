using LocationApp.Account.Hubs;
using LocationApp.Location.Extensions;
using LocationApp.Location.Middleware;
using LocationApp.Services.Middlware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.ConfigureSwagger();
builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.RegisterServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<RequestResponseLoggingMiddleware>();
app.UseMiddleware<ApiKeyMiddleware>();
app.UseMiddleware<IdempotencyMiddleware>();

app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<SearchHub>("/searchHub");
});

app.Run();
