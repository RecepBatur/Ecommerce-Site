using Microsoft.EntityFrameworkCore;
using System.Globalization;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<E_Ticaret.Models.eTicaretContext>(x => x.UseSqlServer("Data Source=RECEP;Initial Catalog=Eticaret;User Id=sa;Password=159753"));
builder.Services.AddDbContext<E_Ticaret.Areas.Admin.Models.UserContext>(x => x.UseSqlServer("Data Source=RECEP;Initial Catalog=Eticaret;User Id=sa;Password=159753"));
builder.Services.AddSession();
var app = builder.Build();
app.UseSession();

CultureInfo culture = new CultureInfo("tr-TR");
culture.NumberFormat.NumberDecimalSeparator = ".";
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
      name: "areas",
      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );
});


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



app.Run();
