using Microsoft.EntityFrameworkCore;
using PapageorgiouHospitalQueueSystem.Data;

namespace PapageorgiouHospitalQueueSystem.Services
{
    public class DeleteCompletedPatientsService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(5); // check every 5 mins
        private readonly TimeSpan _deleteAfter = TimeSpan.FromMinutes(10); // delete after 10 mins

        public DeleteCompletedPatientsService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var threshold = DateTime.UtcNow.Subtract(_deleteAfter);

                    var patientsToDelete = await dbContext.Patients
                        .Where(p => p.Status == 2 && p.CompletedAt != null && p.CompletedAt < threshold)
                        .ToListAsync();

                    if (patientsToDelete.Any())
                    {
                        dbContext.Patients.RemoveRange(patientsToDelete);
                        await dbContext.SaveChangesAsync();
                    }
                }

                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}

