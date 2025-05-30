using Microsoft.EntityFrameworkCore;
using PapageorgiouHospitalQueueSystem.Data;
using PapageorgiouHospitalQueueSystem.Hubs;
using PapageorgiouHospitalQueueSystem.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddSignalR();

builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IDoctorsOfficeService, DoctorsOfficeService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5500", "http://127.0.0.1:5500")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthorization();
app.MapHub<QueueHub>("/ws/waiting-room");

app.MapControllers();
app.UseCors("AllowFrontend");
app.UseRouting();
app.Run();
