using System.Windows.Controls;

namespace PDFReader.Views
{
    /// <summary>
    /// Interaction logic for IndexView.xaml
    /// </summary>
    public partial class IndexView : UserControl
    {
        public IndexView()
        {
            InitializeComponent();
        }

        // Garantir inicialização do CoreWebView2 para evitar exceções ao navegar via Source binding
        private async void PdfWebView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                if (PdfWebView.CoreWebView2 == null)
                {
                    await PdfWebView.EnsureCoreWebView2Async();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Erro inicializando WebView2: " + ex.Message);
            }
        }
    }
}