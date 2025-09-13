using Caliburn.Micro;
using Microsoft.Extensions.Configuration;
using PDFReader.Configuration;
using PDFReader.Infrastructure;
using PDFReader.Logging;
using PDFReader.ViewModels;
using System.IO;
using System.Windows;

namespace PDFReader
{
    public class Bootstrapper : BootstrapperBase
    {
        private readonly SimpleContainer _container = new SimpleContainer();

        public Bootstrapper()
        {
            LoggingConfig.ConfigureLogging();
            Initialize();
        }

        protected override void Configure()
        {
            // Configuração do appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Registro das Configurações
            _container.Instance<IConfiguration>(configuration);

            // Registra os serviços principais do Caliburn.Micro
            _container.Singleton<IWindowManager, WindowManager>();
            _container.Singleton<IEventAggregator, EventAggregator>();

            _container.Singleton<ILoggingService, SerilogLogger>();

            // Registro dos ViewModels
            _container.PerRequest<IndexViewModel, IndexViewModel>();
            _container.PerRequest<ShellViewModel, ShellViewModel>();

            // Registro do ServiceProvider
            DependencyResolver.SetContainer(_container);
        }

        protected override object GetInstance(Type service, string key)
        {
            var instance = _container.GetInstance(service, key);
            if (instance != null) return instance;
            throw new InvalidOperationException($"Não pode resolver {service.Name}");
        }

        protected override async void OnStartup(object sender, StartupEventArgs e)
        {
            await DisplayRootViewForAsync<ShellViewModel>();
        }
    }
}