global using System;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();
builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "DotNet.IMS.Server", Version = "v1" });
});

// Register services
builder.Services.AddSingleton<DotNet.IMS.Server.Services.ITenantValidator, DotNet.IMS.Server.Services.TenantValidator>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

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

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
