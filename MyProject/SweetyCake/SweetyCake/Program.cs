
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.Authorization;
using Serilog;
using SweetyCake.Extensions;

var builder = WebApplication.CreateBuilder(args);





#region Configure Project Services


builder.Services.ConfigureProjectServices(builder.Configuration);

#endregion





// Configure Serilog from appsettings.json
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog(); // Use Serilog for logging

var app = builder.Build();


#region MiddleWares

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

app.UseRouting();

app.UseHttpsRedirection();

app.UseCors("_myAllowSpecificOrigins");
app.UseStaticFiles();


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

#endregion

app.Run();


