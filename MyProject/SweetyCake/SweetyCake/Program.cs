
using Microsoft.AspNetCore.Diagnostics;
using Serilog;
using SweetyCake.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog from appsettings.json
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog(); // Use Serilog for logging

builder.Services.ConfigureProjectServices(builder.Configuration);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseDeveloperExceptionPage();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        var contextFeature = context.Features.Get<IExceptionHandlerFeature>();

        if (contextFeature != null)
        {
            // log in both Development and Production
            Log.Error(contextFeature.Error, "An unhandled exception occurred.");

            //if (app.Environment.IsDevelopment())
            //{
            // If in Development, return detailed error information
            await context.Response.WriteAsJsonAsync(new
            {
                StatusCode = context.Response.StatusCode,
                Message = "Internal Server Error. This is a development environment. Here are the details:",
                Detailed = contextFeature.Error,
            });
            //}
            //else
            //{
            //    // In Production, hide details but return a generic message
            //    await context.Response.WriteAsJsonAsync(new
            //    {
            //        StatusCode = context.Response.StatusCode,
            //        Message = "Internal Server Error. Please check the log file for more details."
            //    });
            //}
        }
    });
});

app.UseHttpsRedirection();

app.UseCors("_myAllowSpecificOrigins");
app.UseStaticFiles();


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}