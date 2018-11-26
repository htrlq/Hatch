namespace LoggerUtil
{
    public class SeriLoggerConfigure
    {
        public bool IsSeq => !string.IsNullOrWhiteSpace(SeqUrl);
        public string SeqUrl { get; set; }
        public bool IsSqlServer => !string.IsNullOrWhiteSpace(ConnectString);
        public string ConnectString { get; set; }
    }
}
