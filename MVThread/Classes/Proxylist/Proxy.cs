using System.Net;
using System.Text.RegularExpressions;

namespace MVThread
{
    public class Proxy
    {
        public Uri Address { get; private set; }
        public NetworkCredential NetworkCredential { get; private set; }

        public Proxy(ProxyType type, string address)
        {
            if(string.IsNullOrEmpty(address))
                throw new ArgumentNullException(nameof(address));

            if (!Regex.IsMatch(address, Constant.ProxyPattern_SingleLine))
                throw new Exception(Constant.Proxylist_AddressException);

            string host = string.Empty;
            string port = string.Empty;
            string username = string.Empty;
            string password = string.Empty;

            string[] array = address.Split(new string[] { Constant.Separator }, StringSplitOptions.RemoveEmptyEntries);
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
                    Address = new UriBuilder($"{Constant.Scheme_Http}://{host}:{port}").Uri;
                    break;
                case ProxyType.Socks4:
                    Address = new UriBuilder($"{Constant.Scheme_Socks4}://{host}:{port}").Uri;
                    break;
                case ProxyType.Socks4a:
                    Address = new UriBuilder($"{Constant.Scheme_Socks4a}://{host}:{port}").Uri;
                    break;
                case ProxyType.Socks5:
                    Address = new UriBuilder($"{Constant.Scheme_Socks5}://{host}:{port}").Uri;
                    break;
                default:
                    Address = new UriBuilder($"{Constant.Scheme_Http}://{host}:{port}").Uri;
                    break;
            }
            NetworkCredential = new NetworkCredential(username, password);
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
    }
}