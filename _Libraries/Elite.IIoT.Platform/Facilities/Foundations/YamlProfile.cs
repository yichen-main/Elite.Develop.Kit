namespace Elite.IIoT.Platform.Facilities.Foundations;
internal sealed class YamlProfile
{
    public TextWindow Window { get; init; } = new();
    public TextOperation Operation { get; init; } = new();
    public TextLogger Logger { get; init; } = new();
    public TextProtocol Protocol { get; init; } = new();
    internal sealed class TextWindow
    {
        public string TitleName { get; init; } = "System";
        public string KanbanName { get; init; } = "Eywa IIoT";
        public int StartingPoint { get; init; } = 3;
        public int WordSpacing { get; init; } = 1;
        public int DividerLength { get; init; } = 72;
    }
    internal sealed class TextOperation
    {
        public string SystemFlag { get; init; } = "eywa";
        public string PlugInAreaName { get; init; } = "Modules";
        public string DateTimeFormat { get; init; } = "yyyy/MM/dd HH:mm:ss";
    }
    internal sealed class TextLogger
    {
        public int EventLevel { get; init; } = 0;
        public int RollingInterval { get; init; } = 3;
        public int RetentionCount { get; init; } = 5;
        public string FilePath { get; init; } = "./Logs";
        public string FileExtension { get; init; } = ".log";
        public string OutputTemplate { get; init; } = "[{Timestamp:HH:mm:ss}] {Message:lj}{Exception}{NewLine}";
    }
    internal sealed class TextProtocol
    {
        public string RootPath { get; init; } = "iiot/service";
        public TextHTTP HTTP { get; init; } = new();
        public TextHTTPS HTTPS { get; init; } = new();
        public TextMQTT MQTT { get; init; } = new();
        internal sealed class TextHTTP
        {
            public int Port { get; init; } = 7260;
        }
        internal sealed class TextHTTPS
        {
            public int Port { get; init; } = 7261;
            public bool Enabled { get; init; }
            public TextCertificate Certificate { get; init; } = new();
            internal sealed class TextCertificate
            {
                public string Location { get; init; } = ".";
                public string Password { get; init; } = "12345";
            }
        }
        internal sealed class TextMQTT
        {
            public int Port { get; init; } = 1883;
        }
    }
}