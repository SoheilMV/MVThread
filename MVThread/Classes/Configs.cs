using System.Threading.Tasks;

namespace MVThread
{
    public delegate Status Config(object sender, DataEventArgs e);
    public delegate Task<Status> ConfigAsync(object sender, DataEventArgs e);
}