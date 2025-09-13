using Caliburn.Micro;
using PDFReader.Infrastructure;
using PDFReader.ViewModels;

namespace PDFReader
{
    public static class ActiveView
    {
        public static ShellViewModel Parent;

        /// <summary>
        /// Realiza a abertura de um viewmodel através do ShellViewModel
        /// </summary>
        public static async Task OpenItem<T>(params object[] args) where T : IScreen
        {
            var viewModel = (T)DependencyResolver.CreateInstance(typeof(T), args);
            await Parent.ActivateItemAsync(viewModel);
        }
    }
}