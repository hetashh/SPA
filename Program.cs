using Microsoft.EntityFrameworkCore;
using SPA.Data;

var builder = WebApplication.CreateBuilder(args);

// Добавление служб MVC
builder.Services.AddControllersWithViews();

// Настройка контекста базы данных
builder.Services.AddDbContext<SpaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Настройка кэширования и сессий
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Время ожидания сессии
    options.Cookie.HttpOnly = true; // Настройка безопасности куки
    options.Cookie.IsEssential = true; // Кука необходима для работы приложения
});

// Добавление HttpContextAccessor для доступа к HttpContext в сервисах
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Включение использования сессий
app.UseSession();

// Настройка обработки ошибок для производственной среды
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Настройка маршрутизации контроллеров
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
