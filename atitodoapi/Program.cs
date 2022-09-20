using Atitodo.Data.Model;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddResponseCompression();

builder.Services.AddDbContext<MainDbContext>(options =>
 options.UseNpgsql(builder.Configuration.GetConnectionString("main")));

Console.WriteLine("Cstr:{0}", builder.Configuration.GetConnectionString("main"));


builder.Services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
{
	builder
	.AllowAnyMethod()
	.AllowAnyHeader().WithOrigins("http://atitodoapp.laky.ovh").WithOrigins("https://atitodoapp.laky.ovh")
	.WithOrigins("http://localhost:5173");
}));


var app = builder.Build();

app.UseResponseCompression();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
