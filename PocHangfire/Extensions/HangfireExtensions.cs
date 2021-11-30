using Aspnetcore.Hangfire.Services;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Aspnetcore.Hangfire.Extensions
{
    public static class HangFireExtensions
    {
        public static IServiceCollection AddHangFireStorageAndServer(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DBConnection");

            //Configurações para colocar o polling dos processos para zero
            var sqlServerStorageOptions = new SqlServerStorageOptions
            {
                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                QueuePollInterval = TimeSpan.Zero
            };

            //Configuração do hangfire
            //add o sql server como job Storage
            //configurando a compatibilidade com a versão 170
            services.AddHangfire(config =>
            {
                config.UseSqlServerStorage(connectionString, sqlServerStorageOptions);
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170);
            });

            //add o server do hangfire
            services.AddHangfireServer();

            return services;
        }


        /// <summary>
        /// Extension para inserir a url customizada e o filtro de autorização
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder ConfigureHangfireDashboard(this IApplicationBuilder app)
        {
            //add filtro de autenticação para a dashboard ** não funcionou ainda
            //app.UseHangfireDashboard("/processos", new DashboardOptions
            //{
            //    Authorization = new[] { new HangfireAuthorizationFilter() },
            //});  

            var options = new DashboardOptions { AppPath = "https://localhost:5001/processos/jobs/enqueued" };

            app.UseHangfireDashboardCustomOptions(new HangfireDashboardCustomOptions
            {
                DashboardTitle = () => "AspnetCore - HangFire",
            });

            app.UseHangfireDashboard("/processos", options);

            //Insere a visibilidade de somente leitura na dashboard sem permissão para deletar ou enfileriar os jobs
            // IsReadOnlyFunc = (DashboardContext context) => true

            return app;
        }

        /// <summary>
        /// Extension que executa os processos presentes na aplicação
        /// </summary>
        /// <param name="app"></param>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static IApplicationBuilder ExecuteJobs(this IApplicationBuilder app, IServiceProvider serviceProvider)
        {
            serviceProvider.GetService<IJobService>().DelayedJob();
            serviceProvider.GetService<IJobService>().FireAndForgetJob();
            serviceProvider.GetService<IJobService>().ContinuationJob();
            serviceProvider.GetService<IJobService>().ReccuringJob();

            return app;
        }
    }
}
