namespace PDFReader.Logging
{
    public interface ILoggingService
    {
        public void LogError(Exception ex, string localException);
    }
}