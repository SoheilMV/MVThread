# MVThread [![Nuget](https://img.shields.io/nuget/v/MVThread)](https://www.nuget.org/packages/MVThread/)
Multi Threading/Tasking in (.NET Framework 4.5+ / .NET Standard 2.0 / .NET 6.0)

#### Features
- Support any lists
- Support two-part lists (usernames-passwords)
- Support proxy lists and links
- Supports list position adjustment
- Show CPM
- Show elapsed time
- Show the number of threads running
- Show progress value

#### Events
- Start
- Stop
- Complete
- Config
- ConfigAsync
- Exception

#### Using
```csharp
static void Main(string[] args)
{
    List<string> list = new List<string>();
    for (int i = 1; i <= 1000; i++) //Adds 1-1000 to the list
    {
        list.Add(i.ToString());
    }
    
    IRunner runner = new TaskRunner();
    runner.OnStarted += Run_OnStarted;
    runner.OnStopped += Run_OnStopped;
    runner.OnCompleted += Run_OnCompleted;
    runner.OnConfig += Run_OnConfig;
    runner.OnConfigAsync += Run_OnConfigAsync;

    runner.SetWordlist(list); //Add list to runner
    runner.Start(2); //Add bot count in runner and start the runner
    
    while (runner.IsRunning)
    {
        Console.Title = $"Bot : {runner.Active} - CPM : {runner.CPM} - Elapsed : {runner.Elapsed} ";
        Thread.Sleep(100);
    }

    Console.ReadKey();
}

private static void Run_OnStarted(object sender, EventArgs e)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Started!"); //Displays the start message when the runner start
}

private static void Run_OnStopped(object sender, StopEventArgs e)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Stopped!"); //Displays the stop message when the runner stop
}

private static void Run_OnCompleted(object sender, EventArgs e)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("Completed!"); //Displays the completed message when the runner complete
}

private static Status Run_OnConfig(object sender, DataEventArgs e)
{
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine(e.Data); //Display the value received from the list
    return Status.OK;
}

private static async Task<Status> Run_OnConfigAsync(object sender, DataEventArgs e)
{
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine(e.Data); //Display the value received from the list
    return Status.OK;
}
```

#### How to add a combolist to the runner?
```csharp
List<string> list = new List<string>();

runner.SetWordlist(list);
```

#### How to add 2-part lists to the runner?
```csharp
List<string> usernames = new List<string>();
List<string> passwords = new List<string>();

runner.SetWordlist(usernames, passwords, ComboType.ChangeUser); //u1:p1-u2:p1
//or
runner.SetWordlist(usernames, passwords, ComboType.ChangePass); //u1:p1-u1:p2
```


#### How to set the list position when you start?
```csharp
runner.SetWordlist(list, number);
//or
runner.SetWordlist(list, list, type, number);
```

#### How to add a proxylist to your runner?
```csharp
List<string> proxies = new List<string>();
runner.SetProxylist(proxies, type);

//or

string url = "https://localhost/proxies/";
var proxies = runner.GetProxylist(url);
runner.SetProxylist(proxies, type);

//or

string path = "C:\\Proxylist.txt";
var proxies = runner.GetProxylist(path);
runner.SetProxylist(proxies, type);
```

#### Overview of the configuration event
```csharp
private static void Run_OnConfig(object sender, DataEventArgs e)
{
    string data = e.Data; //Get data from the entered list
    
    if(e.Retry > 100) //Shows the current data retrieval count
        return Status.TheEnd; //In certain circumstances, you can stop all the threads if you wish
        
    if (!e.ProxyDetail.IsProxyLess) //Get a proxy at random
    {
        Uri address = e.ProxyDetail.Proxy.Address;
        ICredentials credentials = e.ProxyDetail.Proxy.NetworkCredential;
        ProxyType proxyType = e.ProxyDetail.Proxy.GetProxyType();
    }
    
    try
    {
        e.Save.WriteLine("goods.txt", data); //Save data in a file
        return Status.OK; //To get the continuation of the list return Status.OK
    }
    catch (Exception ex)
    {
        e.Log.WriteLine(ex.Message); //You can save your messages as a log
        return Status.Retry; //If the operation fails, you can retrieve the current data
    }
}
```
