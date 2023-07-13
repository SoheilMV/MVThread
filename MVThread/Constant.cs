namespace MVThread
{
    internal class Constant
    {
        public const string ElapsedFormat = "{0:00}:{1:00}:{2:00}:{3:00}";

        public const string Log_FileName = "Log.txt";
        public const string Log_DateTimeFormat = "yyyy/MM/dd hh:mm:ss";
        public const string Log_DateTime = "<DateTime>";
        public const string Log_Value = "<Value>";
        public const string Log_Format = $"{Log_DateTime} - {Log_Value}";

        public const string UrlPattern = @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?";
        public const string DomainPattern = @"(?:[a-zA-Z0-9-]+\.)+[a-zA-Z]{2,}";
        public const string IPv4Pattern = @"(?:[0-9]{1,3}\.){3}[0-9]{1,3}";
        public const string IPv6Pattern = @"(?:[A-Fa-f0-9]{1,4}:){7}[A-Fa-f0-9]{1,4}";
        public const string ProxyPattern = @"((?:[a-zA-Z0-9-]+\.)+[a-zA-Z]{2,}|(?:[0-9]{1,3}\.){3}[0-9]{1,3}|(?:[A-Fa-f0-9]{1,4}:){7}[A-Fa-f0-9]{1,4}):(\d+)(?:[^:]|:([\w-!@#$%^&*()_+=;,.<>?\'\""/\\\[\]\{\}]+)(?::([\w-!@#$%^&*()_+=;,.<>?\'\""/\\\[\]\{\}]+))?)?";

        public const string Wordlist_Separator = ":";
        public const string Wordlist_ArgumentNullException = "Wordlist is null.";
        public const string Wordlist_Position_ArgumentNullException = "Position 0-<Count>";
        public const string Proxylist_ArgumentNullException = "Proxylist is null.";
        public const string Proxylist_FileException = "Cannot access your file.";
        public const string Proxylist_LinkException = "Please enter valid proxy link and try.";
        public const string Proxylist_AddressException = "The address format is incorrect.";

        public const string OnConfigAsync_Exception = "OnConfigAsync event cannot be null.";
        public const string OnConfig_Exception = "OnConfig event cannot be null.";

        public const string Scheme_Http = "http";
        public const string Scheme_Socks4 = "socks4";
        public const string Scheme_Socks4a = "socks4a";
        public const string Scheme_Socks5 = "socks5";
        public const string Scheme_Exception = "The type is unknown.";
    }
}
