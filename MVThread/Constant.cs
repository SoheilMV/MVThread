namespace MVThread
{
    internal class Constant
    {
        public const string Separator = ":";
        public const string ElapsedFormat = "{0:00}:{1:00}:{2:00}:{3:00}";

        public const string UrlPattern = @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?";
        public const string ProxyPattern_MultiLine = @"([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?:(\d+)(?:[^:]|:([\w-]+)(?::([\w-]+))?)?"; //https://regex101.com/r/ZmltXj/1
        public const string ProxyPattern_SingleLine = $"^{ProxyPattern_MultiLine}$"; //https://regex101.com/r/ZmltXj/2

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
