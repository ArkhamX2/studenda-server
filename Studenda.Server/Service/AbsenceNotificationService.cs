using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Studenda.Server.Data;
using Studenda.Server.Service.Journal;
using Studenda.Server.Service.Security;

namespace Studenda.Server.Service
{
    /// <summary>
    /// Фоновый сервис для рассылки писем родителям студентов с большим количеством пропусков.
    /// </summary>
    public class AbsenceNotificationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly EmailService _emailService;

        public AbsenceNotificationService(IServiceProvider serviceProvider, EmailService emailService)
        {
            _serviceProvider = serviceProvider;
            _emailService = emailService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.Now;
                // рассылка 1 февраля и 1 июля
                if ((now.Month == 2 && now.Day == 1) || (now.Month == 7 && now.Day == 1))
                {
                    await NotifyParentsAsync();
                }
                // Проверять раз в сутки
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }

        private async Task NotifyParentsAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DataContext>();

            var students = await db.Accounts
                .Where(a => a.ParentEmail != null && a.ParentEmail != "")
                .Include(a => a.Absences)
                .ToListAsync();

            foreach (var student in students)
            {
                int absenceCount = student.Absences.Count;
                if (absenceCount > 10)
                {
                    string subject = "Внимание: Пропуски занятий";
                    string body = $"У студента {student.Surname} {student.Name} за прошлый период накопилось {absenceCount} пропусков. Пожалуйста, обратите внимание.";
                    await _emailService.SendAsync(student.ParentEmail, subject, body);
                }
            }
        }
    }
}
