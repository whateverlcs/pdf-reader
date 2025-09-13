using Serilog;

namespace PDFReader.Logging
{
    public class SerilogLogger : ILoggingService
    {
        public void LogError(Exception ex, string localException)
        {
            Log.Error(ex, $"{DateTime.Now} | Local: {localException}\n");
        }
    }
}