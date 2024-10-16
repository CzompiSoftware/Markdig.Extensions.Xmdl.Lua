using Markdig.Extensions.Xmdl.ExecutableCode;
using Microsoft.Extensions.Logging;
using System.Runtime.Loader;

namespace Markdig.Extensions.Xmdl.Lua;

public class ExecutableCodeOptions : IExecutableCodeOptions
{
    public bool IsDebugMode => (int)MinimumLogLevel <= (int)LogLevel.Debug;

    public LogLevel MinimumLogLevel { get; init; } = LogLevel.Warning;

    public string WorkingDirectory { get; init; }

    public string[] Arguments { get; init; }

    //public OptimizationLevel? OptimizationLevel { get; init; }

    public bool NoCache { get; init; } = false;

    public string[] References { get; init; }

    public string[] Usings { get; init; }

    public AssemblyLoadContext AssemblyLoadContext { get; init; }

}
