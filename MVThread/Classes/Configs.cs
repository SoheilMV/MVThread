using System.Threading.Tasks;

namespace MVThread
{
    public delegate ConfigStatus Config(object sender, DataEventArgs e);
    public delegate Task<ConfigStatus> ConfigAsync(object sender, DataEventArgs e);
}