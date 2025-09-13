using MahApps.Metro.Controls;
using System.Windows;

namespace PDFReader.Views
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView : MetroWindow
    {
        public ShellView()
        {
            InitializeComponent();
        }

        public void MaximizarJanela()
        {
            this.WindowState = WindowState.Maximized;
        }
    }
}