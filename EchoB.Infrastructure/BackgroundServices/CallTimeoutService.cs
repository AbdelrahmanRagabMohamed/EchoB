using EchoB.Domain.Enums;
using EchoB.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EchoB.Infrastructure.BackgroundServices
{
    public class CallTimeoutService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public CallTimeoutService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var callRepository = scope.ServiceProvider.GetRequiredService<ICallRepository>();
                    var signalRService = scope.ServiceProvider.GetRequiredService<ISignalRService>();

                    var pendingCalls = await callRepository.GetPendingCallsAsync();

                    foreach (var call in pendingCalls.Where(c => (DateTime.UtcNow - c.StartTime).TotalSeconds > 60))
                    {
                        call.Status = CallStatus.Missed;
                        call.EndTime = DateTime.UtcNow;

                        await callRepository.UpdateAsync(call);
                        await signalRService.NotifyCallEnded(call.CallerId.ToString(), call.ReceiverId.ToString());
                    }
                }

                await Task.Delay(10000, stoppingToken); // Check every 10 seconds
            }
        }
    }
}
