namespace LoggerUtil
{
    public class SeriLoggerConfigure
    {
        public bool IsSeq => Seq != null;
        public SeqConfiguration Seq { get; set; }
        public bool IsSqlServer => !string.IsNullOrWhiteSpace(ConnectString);
        public string ConnectString { get; set; }
    }

    public class SeqConfiguration
    {
        public string Url { get; set; }
        public string ApiKey { get; set; }

        public static implicit operator SeqConfiguration(string url)
        {
            return new SeqConfiguration() {Url = url};
        }
    }
}
