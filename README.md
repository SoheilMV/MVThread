# MVThread
Multi-trading library in .NET

#### Features
- Supports any list
- Supports userlist-passlist
- Supports list position adjustment

#### Events
- Start
- Stop
- Complete
- Config
- Exception

#### Using
```csharp
private static List<string> list = new List<string>();

static void Main(string[] args)
{
	for (int i = 1; i <= 10; i++) //Adds 1-10 to the list
	{
		list.Add(i.ToString());
	}

	Runner run = new Runner();
	run.OnStarted += Run_OnStarted;
	run.OnStopped += Run_OnStopped;
	run.OnCompleted += Run_OnCompleted;
	run.OnConfig += Run_OnConfig;
	
	run.SetWordlist(list); //Add list to runner
	run.Start(2); //Add bot count in runner and start the runner
	
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

	Console.ForegroundColor = ConsoleColor.Yellow;
	Console.WriteLine("Completed!"); //Show end message
}

private static void Run_OnConfig(object sender, DataEventArgs e)
{
	Console.ForegroundColor = ConsoleColor.White;
	Console.WriteLine(e.Data); //Display the value received from the list
}
```
