using System;
using System.Net;
using System.Text.RegularExpressions;

namespace MVThread
{
    public class Proxy
    {
        public Proxy(ProxyType type, string address)
        {
            if(string.IsNullOrEmpty(address))
                throw new ArgumentNullException("address");

            string proxyPattern = @"^(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}):(\d+)(?::(\w+))?(?::(\w+))?$"; //https://regex101.com/r/ZmltXj/2
            if (!Regex.IsMatch(address, proxyPattern))
                throw new Exception("The address format is incorrect.");

            string host = string.Empty;
            string port = string.Empty;
            string username = string.Empty;
            string password = string.Empty;

            string[] array = address.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
            if (array.Length == 2)
            {
                host = array[0];
                port = array[1];
            }
            else if (array.Length == 3)
            {
                host = array[0];
                port = array[1];
                username = array[2];
            }
            if (array.Length == 4)
            {
                host = array[0];
                port = array[1];
                username = array[2];
                password = array[3];
            }
                

            switch (type)
            {
                case ProxyType.Http:
                    Address = new UriBuilder($"http://{host}:{port}").Uri;
                    break;
                case ProxyType.Socks4:
                    Address = new UriBuilder($"socks4://{host}:{port}").Uri;
                    break;
                case ProxyType.Socks4a:
                    Address = new UriBuilder($"socks4a://{host}:{port}").Uri;
                    break;
                case ProxyType.Socks5:
                    Address = new UriBuilder($"socks5://{host}:{port}").Uri;
                    break;
                default:
                    Address = new UriBuilder($"http://{host}:{port}").Uri;
                    break;
            }
            NetworkCredential = new NetworkCredential(username, password);
        }

        public Uri Address { get; private set; }
        public NetworkCredential NetworkCredential { get; private set; }

        public ProxyType GetProxyType()
        {
            if(Address.Scheme == "http")
                return ProxyType.Http;
            else if(Address.Scheme == "socks4")
                return ProxyType.Socks4;
            else if (Address.Scheme == "socks4a")
                return ProxyType.Socks4a;
            else if (Address.Scheme == "socks5")
                return ProxyType.Socks5;
            else
                throw new Exception("The type is unknown.");
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(NetworkCredential.UserName) && NetworkCredential.SecurePassword.Length > 0)
                return $"{Address.Host}:{Address.Port}:{NetworkCredential.UserName}:{NetworkCredential.Password}";
            else
                return $"{Address.Host}:{Address.Port}";
        }
    }
}