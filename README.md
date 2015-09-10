# WurmApi #
----------------

WurmApi is a .NET library, realizing mission to enable other programmers, to easily write their own tools for Wurm Online game client and for the game in general.

## Usage ##

### Using in your .NET project

A - Simple:

1. Clone this repository.
2. Build WurmApi in release mode.
3. Reference all output DLL in your application.

B - Alternative:

1. Clone this repository
2. Add all WurmApi projects to your solution.
3. Reference WurmApi projects in your projects.

*If copying DLLs, include AldursLab.WurmApi.XML for intellisense docs.*

### Namespace ###

	using AldursLab.WurmApi;

### Creation ###

Using all defaults:

    using (IWurmApi api = WurmApiFactory.Create())
    {
        // use the API
    }

##### Setting custom data directory ####

Relative to WurmApi.dll:

    IWurmApi api = WurmApiFactory.Create("WurmApiData");

With absolute path:

	IWurmApi api = WurmApiFactory.Create("C:\WurmApps\WurmApi");

*Data directory is used to store data caches, which speed up next API calls for similar data.*

*Do not use same directory for multiple concurrent instances of WurmApi.*

##### Getting log outputs ###

Implement logging interface:

	class WurmApiLogger : ILogger
    {
        public void Log(LogLevel level, string message, object source, Exception exception)
        {
            Console.WriteLine("{0}, {1}, {2}, {3}", level, message, source, exception);
        }
    }

Use in construction:

	IWurmApi api = WurmApiFactory.Create("WurmApiData", new WurmApiLogger());

*WurmApi will log any suspicious or error state during it's internal updates. Unless logging is set, these messages will go nowhere and debugging issues will be harder.*

##### Marshalling events to GUI thread ###

Implement event marshalling interface:

*Example implementation for a WinForms application, executing all events on UI thread of MainForm.*

    class EventMarshaller : IEventMarshaller
    {
        public void Marshal(Action action)
        {
            Program.MainForm.BeginInvoke(action);
        }
    }

Use in construction:

	IWurmApi api = WurmApiFactory.Create("WurmApiData", new WurmApiLogger(), new EventMarshaller());

*WurmApi by default runs its events on ThreadPool, which may complicate event handling. By marshalling them to appropriate thread (most often, GUI thread), apps can remain single-threaded and still leverage WurmApi events.*

##### Overriding Wurm Client directory ###

If autodetection doesn't work for Wurm Client install directory, it can be overriden.

Implement install directory interface:

	class WurmInstallDir : IWurmClientInstallDirectory
    {
        public string FullPath { get { return @"C:\games\Wurm"; } }
    }

Use in construction:

	IWurmApi api = WurmApiFactory.Create("WurmApiData", new WurmApiLogger(), new EventMarshaller(), new WurmInstallDir());

### API quick examples

##### Add commands to autoruns #

    var command = @"say /time";
    api.Autoruns.MergeCommandToAllAutoruns(command);

##### Get servers for a game character:

    var character = api.Characters.Get("Batman");

    var currentServer = character.GetCurrentServer();
	var currentServerUptime = currentServer.TryGetCurrentTime();

    var serverOneDayAgo = character.GetHistoricServerAtLogStamp(DateTime.Now.Subtract(TimeSpan.FromDays(1)));

##### Search log files

    var scanResults = api.LogsHistory.Scan(new LogSearchParameters()
    {
        CharacterName = new CharacterName("Batman"),
        DateFrom = DateTime.Now.Subtract(TimeSpan.FromDays(5)),
        DateTo = DateTime.Now,
        LogType = LogType.Event,
    });
    var digsCount = scanResults.Count(entry => entry.Content.Contains("You dig a hole."));

##### Subscribe to live events

    int digsCount = 0;
    EventHandler<LogsMonitorEventArgs> eventHandler = (sender, eventArgs) =>
    {
        digsCount += eventArgs.WurmLogEntries.Count(entry => entry.Content.Contains("You dig a hole."));
    };
    var batman = new CharacterName("Batman");
    api.LogsMonitor.Subscribe(batman, LogType.Event, eventHandler);
    Console.ReadKey();
	Console.WriteLine("You have dug " + digsCount + " holes.");
    api.LogsMonitor.Unsubscribe(batman, eventHandler);

##### Read game client config values

	var batmanSkillgainRateSetting = api.Configs.GetConfig("default").SkillGainRate;

### Notes ###

WurmApi should always be Disposed, to ensure cleanup and consistency of data caches. 

WurmApi is thread safe and a single instance is enough to handle entire app requirements.

Expensive methods have async and cancellable overloads.

Exact behavior and corner cases are documented on the API itself (xml docs).

## To-Do ##

1. Mono builds for all major desktop platforms.
2. Cross-platform Java bindings for the Mono build.

## Contribution ##

WurmApi is an opensource project. It is shared, so that other developers can not only use it, but continously improve it. Feedback is welcome, if there are features needed, they can be added. Feel free to ask any questions.