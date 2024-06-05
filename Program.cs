using Microsoft.EntityFrameworkCore;
using SPA.Data;

var builder = WebApplication.CreateBuilder(args);

// ���������� ����� MVC
builder.Services.AddControllersWithViews();

// ��������� ��������� ���� ������
builder.Services.AddDbContext<SpaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ��������� ����������� � ������
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // ����� �������� ������
    options.Cookie.HttpOnly = true; // ��������� ������������ ����
    options.Cookie.IsEssential = true; // ���� ���������� ��� ������ ����������
});

// ���������� HttpContextAccessor ��� ������� � HttpContext � ��������
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// ��������� ������������� ������
app.UseSession();

// ��������� ��������� ������ ��� ���������������� �����
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// ��������� ������������� ������������
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
