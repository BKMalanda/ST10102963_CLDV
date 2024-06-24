using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ST10102963_CLDV.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.WebJobs.Extensions.DurableTask.ContextImplementations;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ST10102963_CLDVContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ST10102963_CLDVContext") ?? throw new InvalidOperationException("Connection string 'ST10102963_CLDVContext' not found.")));

/* code reference: Official Azure Storage Blobs client library for .NET:
https://learn.microsoft.com/en-us/dotnet/api/overview/azure/storage.blobs-readme */

// Configure Azure services
builder.Services.AddAzureClients(azureBuilder =>
{
    azureBuilder.AddBlobServiceClient(builder.Configuration.GetConnectionString("AzureWebJobsStorage"));
    azureBuilder.AddQueueServiceClient(builder.Configuration.GetConnectionString("AzureWebJobsStorage"));
});

// Configure Durable Functions
builder.Services.AddDurableClientFactory(options =>
{
    options.ConnectionName = "AzureWebJobsStorage";
    options.TaskHub = "ST10102963_CLDV_TaskHub"; 
});

builder.Services.AddHttpClient();

/* Code reference: https://youtu.be/Y6DCP-yH-9Q?si=sYLQDTcw1aE4Tfwl */

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ST10102963_CLDVContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenari os, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();;


app.MapRazorPages();


app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


/* Code reference: https://youtu.be/Y6DCP-yH-9Q?si=sYLQDTcw1aE4Tfwl */
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "Admin", "Client" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
            Console.WriteLine($"Role '{role}' created successfully."); // Add this line
        }
    }
}

app.Run();
