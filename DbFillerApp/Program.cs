
using DbFillerApp;
using Hangfire.SqlServer;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using DbFillerApp.Data;

var service = new MovieServices();



GlobalConfiguration.Configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseColouredConsoleLogProvider()
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage("Server= localhost;Database=MoviesDb;Trusted_Connection=True;MultipleActiveResultSets=true", new SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromSeconds(5),
        SlidingInvisibilityTimeout = TimeSpan.FromSeconds(5),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        UsePageLocksOnDequeue = true,
        DisableGlobalLocks = true
    });

BackgroundJob.Enqueue(() => Console.WriteLine("Hello, world!"));



RecurringJob.AddOrUpdate(
        () => service.SaveMoviesIntoDb(),
    Cron.Minutely);

using (var server = new BackgroundJobServer())
{
    Console.ReadLine();
}

