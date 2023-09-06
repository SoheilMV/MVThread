using System.Net;
using System.Text.RegularExpressions;

namespace MVThread
{
    public class Proxy
    {
        public Uri Address { get; private set; }
        public NetworkCredential NetworkCredential { get; private set; }
        public string Host { get; private set; } = string.Empty;
        public string Port { get; private set; } = string.Empty;
        public string Password { get; private set; } = string.Empty;
        public string Username { get; private set; } = string.Empty;
        public bool HostIsDomain { get; private set; }
        public bool HostIsIPv4 { get; private set; }
        public bool HostIsIPv6 { get; private set; }

        public Proxy(ProxyType type, string address)
        {
            if (string.IsNullOrEmpty(address))
                throw new ArgumentNullException(nameof(address));

            ParseWithRegex(type, address);
        }

        public ProxyType GetProxyType()
        {
            if(Address.Scheme == Constant.Scheme_Http)
                return ProxyType.Http;
            else if(Address.Scheme == Constant.Scheme_Socks4)
                return ProxyType.Socks4;
            else if (Address.Scheme == Constant.Scheme_Socks4a)
                return ProxyType.Socks4a;
            else if (Address.Scheme == Constant.Scheme_Socks5)
                return ProxyType.Socks5;
            else
                throw new Exception(Constant.Scheme_Exception);
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(NetworkCredential.UserName) && NetworkCredential.SecurePassword.Length > 0)
                return $"{Address.Host}:{Address.Port}:{NetworkCredential.UserName}:{NetworkCredential.Password}";
            else
                return $"{Address.Host}:{Address.Port}";
        }

        private void ParseWithRegex(ProxyType type, string address)
        {
            if (!ProxyUtility.ProxyRegex.IsMatch(address))
                throw new Exception(Constant.Proxylist_AddressException);

            Match proxyMatch = ProxyUtility.ProxyRegex.Match(address);

            Host = proxyMatch.Groups[1].Value;
            Port = proxyMatch.Groups[2].Value;
            Username = proxyMatch.Groups[3].Value;
            Password = proxyMatch.Groups[4].Value;

            HostIsDomain = Regex.IsMatch(Host, Constant.DomainPattern, RegexOptions.Compiled);
            HostIsIPv4 = Regex.IsMatch(Host, Constant.IPv4Pattern, RegexOptions.Compiled);
            HostIsIPv6 = Regex.IsMatch(Host, Constant.IPv6Pattern, RegexOptions.Compiled);

            switch (type)
            {
                case ProxyType.Http:
                    if (HostIsIPv6)
                        Address = new UriBuilder($"{Constant.Scheme_Http}://[{Host}]:{Port}").Uri;
                    else
                        Address = new UriBuilder($"{Constant.Scheme_Http}://{Host}:{Port}").Uri;
                    break;
                case ProxyType.Socks4:
                    if (HostIsIPv6)
                        Address = new UriBuilder($"{Constant.Scheme_Socks4}://[{Host}]:{Port}").Uri;
                    else
                        Address = new UriBuilder($"{Constant.Scheme_Socks4}://{Host}:{Port}").Uri;
                    break;
                case ProxyType.Socks4a:
                    if (HostIsIPv6)
                        Address = new UriBuilder($"{Constant.Scheme_Socks4a}://[{Host}]:{Port}").Uri;
                    else
                        Address = new UriBuilder($"{Constant.Scheme_Socks4a}://{Host}:{Port}").Uri;
                    break;
                case ProxyType.Socks5:
                    if (HostIsIPv6)
                        Address = new UriBuilder($"{Constant.Scheme_Socks5}://[{Host}]:{Port}").Uri;
                    else
                        Address = new UriBuilder($"{Constant.Scheme_Socks5}://{Host}:{Port}").Uri;
                    break;
                default:
                    if (HostIsIPv6)
                        Address = new UriBuilder($"{Constant.Scheme_Http}://[{Host}]:{Port}").Uri;
                    else
                        Address = new UriBuilder($"{Constant.Scheme_Http}://{Host}:{Port}").Uri;
                    break;
            }
            NetworkCredential = new NetworkCredential(Username, Password);
        }
    }

    internal static class ProxyUtility
    {
        public static readonly Regex _proxyRegex = new Regex(Constant.ProxyPattern, RegexOptions.Compiled);

        public static Regex ProxyRegex => _proxyRegex;
    }
}