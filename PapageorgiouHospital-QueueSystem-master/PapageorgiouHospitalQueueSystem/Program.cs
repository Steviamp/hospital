using Microsoft.EntityFrameworkCore;
using PapageorgiouHospitalQueueSystem.Data;
using PapageorgiouHospitalQueueSystem.Helpers;
using PapageorgiouHospitalQueueSystem.Hubs;
using PapageorgiouHospitalQueueSystem.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.

builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IDoctorsOfficeService, DoctorsOfficeService>();
builder.Services.AddHostedService<DeleteCompletedPatientsService>();
builder.Services.AddScoped<IAesEncryptionHelper, AesEncryptionHelper>();
builder.Services.Configure<EncryptionSettings>(
    builder.Configuration.GetSection("Encryption"));



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

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .SetIsOriginAllowed(origin => true); // Allow all for dev
    });
});

builder.Services.AddSignalR();
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

app.MapControllers();
app.UseCors("AllowFrontend");
app.UseCors();
app.UseRouting();
app.MapHub<QueueHub>("/ws/waiting-room");
app.Run();
