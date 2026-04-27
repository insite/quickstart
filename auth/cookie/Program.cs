var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseDefaultFiles(); // Look for index.html by default
app.UseStaticFiles();  // Enable serving static files from wwwroot
app.Run();