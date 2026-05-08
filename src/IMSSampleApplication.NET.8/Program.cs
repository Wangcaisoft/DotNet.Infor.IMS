using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Logging configuration uses the built-in providers (Console).
builder.Services.AddLogging();

// Add controllers and configure Newtonsoft JSON compatibility
builder.Services.AddControllers().AddNewtonsoftJson();

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "IMSSampleApplication API", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Global exception handling middleware
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.ContentType = "application/json";
        var error = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;
        var result = System.Text.Json.JsonSerializer.Serialize(new { message = "An unexpected error occurred.", detail = error?.Message });
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync(result);
    });
});

app.UseRouting();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "IMSSampleApplication API V1"));

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
