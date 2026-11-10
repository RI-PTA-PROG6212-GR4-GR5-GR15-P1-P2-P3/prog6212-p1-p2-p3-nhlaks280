
using RaceDay.Mvc.Services;

var builder = WebApplication.CreateBuilder(args);

// Register MVC controllers and views.
builder.Services.AddControllersWithViews();

// Configure session storage.
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Register the API service.
builder.Services.AddHttpClient<ApiService>(client =>
{
    var apiUrl = builder.Configuration["ApiSettings:BaseUrl"];

    if (string.IsNullOrWhiteSpace(apiUrl))
    {
        throw new InvalidOperationException(
            "API BaseUrl is missing from appsettings.json.");
    }

    client.BaseAddress = new Uri(apiUrl);
});

// Register the image upload service.
builder.Services.AddHttpClient<ImageUploadService>(client =>
{
    var apiUrl = builder.Configuration["ApiSettings:BaseUrl"];

    if (string.IsNullOrWhiteSpace(apiUrl))
    {
        throw new InvalidOperationException(
            "API BaseUrl is missing from appsettings.json.");
    }

    client.BaseAddress = new Uri(apiUrl);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Session must run before controllers access session data.
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
