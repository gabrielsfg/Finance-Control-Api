using FinanceControl.Data.Data;
using FinanceControl.Domain.Entities;
using FinanceControl.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace FinanceControl.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;

    public Worker(ILogger<Worker> logger, IServiceScopeFactory scopeFactory, IConfiguration configuration)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("RecurringTransaction Worker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            var delay = GetDelayUntilNextRun();
            _logger.LogInformation("Next run scheduled in {Minutes} minutes ({RunAt} SP time).",
                (int)delay.TotalMinutes, GetNextRunLocalTime().ToString("yyyy-MM-dd HH:mm:ss"));

            await Task.Delay(delay, stoppingToken);

            if (!stoppingToken.IsCancellationRequested)
                await RunJobAsync(stoppingToken);
        }
    }

    private async Task RunJobAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("RecurringTransaction job started at {Time}.", DateTime.UtcNow);

        int generated = 0;
        int errors = 0;

        try
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var today = GetTodayInLocalTime();

            // Desativa automaticamente as recorrentes cuja EndDate já passou
            var expired = await context.RecurringTransactions
                .Where(rt => rt.IsActive && rt.EndDate != null && rt.EndDate < today)
                .ToListAsync(stoppingToken);

            foreach (var rt in expired)
                rt.IsActive = false;

            if (expired.Count > 0)
            {
                await context.SaveChangesAsync(stoppingToken);
                _logger.LogInformation("Deactivated {Count} expired recurring transactions.", expired.Count);
            }

            var recurringTransactions = await context.RecurringTransactions
                .Where(rt => rt.IsActive)
                .ToListAsync(stoppingToken);

            _logger.LogInformation("Found {Count} active recurring transactions to process.", recurringTransactions.Count);

            foreach (var rt in recurringTransactions)
            {
                try
                {
                    var targetDate = GetTargetDateForToday(rt, today);
                    if (targetDate is null)
                        continue;

                    // Idempotência: já existe transação gerada por este recorrente para este período?
                    var alreadyExists = await context.Transactions.AnyAsync(
                        t => t.RecurringTransactionId == rt.Id && t.TransactionDate == targetDate.Value,
                        stoppingToken);

                    if (alreadyExists)
                        continue;

                    var transaction = new Transaction
                    {
                        UserId = rt.UserId,
                        RecurringTransactionId = rt.Id,
                        AccountId = rt.AccountId,
                        SubCategoryId = rt.SubCategoryId,
                        BudgetId = rt.BudgetId,
                        Value = rt.Value,
                        Type = rt.Type,
                        Description = rt.Description,
                        TransactionDate = targetDate.Value,
                        PaymentType = EnumPaymentType.Recurring
                    };

                    context.Transactions.Add(transaction);
                    generated++;
                }
                catch (Exception ex)
                {
                    errors++;
                    _logger.LogError(ex, "Error processing RecurringTransaction Id={Id}.", rt.Id);
                }
            }

            await context.SaveChangesAsync(stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error in RecurringTransaction job.");
            errors++;
        }

        _logger.LogInformation(
            "RecurringTransaction job finished. Generated={Generated}, Errors={Errors}.",
            generated, errors);
    }

    /// <summary>
    /// Verifica se hoje é o dia correto para gerar uma instância desta recorrente.
    /// Retorna a data-alvo ou null se não deve gerar hoje.
    /// </summary>
    private static DateOnly? GetTargetDateForToday(RecurringTransaction rt, DateOnly today)
    {
        if (today < rt.StartDate)
            return null;

        return rt.Recurrence switch
        {
            EnumRecurrenceType.Daily => today,

            EnumRecurrenceType.WorkDay => IsWorkDay(today) ? today : null,

            EnumRecurrenceType.Weekly =>
                (today.ToDateTime(TimeOnly.MinValue) - rt.StartDate.ToDateTime(TimeOnly.MinValue)).Days % 7 == 0 ? today : null,

            EnumRecurrenceType.Biweekly =>
                (today.ToDateTime(TimeOnly.MinValue) - rt.StartDate.ToDateTime(TimeOnly.MinValue)).Days % 14 == 0 ? today : null,

            EnumRecurrenceType.Monthly =>
                today.Day == GetEffectiveDayForMonth(rt.StartDate.Day, today.Year, today.Month) ? today : null,

            EnumRecurrenceType.Quarterly =>
                today.Day == GetEffectiveDayForMonth(rt.StartDate.Day, today.Year, today.Month) &&
                ((today.Year * 12 + today.Month) - (rt.StartDate.Year * 12 + rt.StartDate.Month)) % 3 == 0
                    ? today : null,

            EnumRecurrenceType.Semiannually =>
                today.Day == GetEffectiveDayForMonth(rt.StartDate.Day, today.Year, today.Month) &&
                ((today.Year * 12 + today.Month) - (rt.StartDate.Year * 12 + rt.StartDate.Month)) % 6 == 0
                    ? today : null,

            EnumRecurrenceType.Annually =>
                today.Day == GetEffectiveDayForMonth(rt.StartDate.Day, today.Year, today.Month) &&
                today.Month == rt.StartDate.Month ? today : null,

            _ => null
        };
    }

    private static bool IsWorkDay(DateOnly date)
        => date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday;

    /// <summary>
    /// Retorna o dia efetivo para o mês/ano alvo.
    /// Ex: startDay=31, fevereiro/2025 → retorna 28.
    /// </summary>
    private static int GetEffectiveDayForMonth(int startDay, int year, int month)
    {
        var daysInMonth = DateTime.DaysInMonth(year, month);
        return Math.Min(startDay, daysInMonth);
    }

    private TimeSpan GetDelayUntilNextRun()
    {
        var nextRun = GetNextRunLocalTime();
        var nowLocal = GetNowInLocalTime();
        var delay = nextRun - nowLocal;
        return delay <= TimeSpan.Zero ? TimeSpan.FromMinutes(1) : delay;
    }

    private DateTime GetNextRunLocalTime()
    {
        var scheduledTime = TimeSpan.Parse(
            _configuration["WorkerSettings:ScheduledTimeLocal"] ?? "00:00:00");

        var tz = GetTimeZone();
        var nowLocal = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
        var todayRun = nowLocal.Date + scheduledTime;

        return nowLocal < todayRun ? todayRun : todayRun.AddDays(1);
    }

    private DateOnly GetTodayInLocalTime()
    {
        var tz = GetTimeZone();
        return DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz));
    }

    private DateTime GetNowInLocalTime()
    {
        var tz = GetTimeZone();
        return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
    }

    private TimeZoneInfo GetTimeZone()
    {
        var tzId = _configuration["WorkerSettings:TimeZone"] ?? "E. South America Standard Time";
        return TimeZoneInfo.FindSystemTimeZoneById(tzId);
    }
}
