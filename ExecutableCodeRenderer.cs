using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using CzomPack.Logging;
using CzomPack.Network;
using Markdig.Extensions.Xmdl.ExecutableCode;
using Markdig.Extensions.Xmdl.Lua.Helper;
using MoonSharp.Interpreter;

namespace Markdig.Extensions.Xmdl.Lua;

internal class ExecutableCodeRenderer(IExecutableCodeOptions options, MarkdownPipeline pipeline) : ExecutableCode.ExecutableCodeRenderer(options, pipeline)
{
    private readonly IExecutableCodeOptions _options = options;
    private readonly MarkdownPipeline _pipeline = pipeline;

    public override async Task<(ExecutableCodeState, ExecutableCodeResult, List<string>)> ExecuteAsync(string script, MarkdownParserContext context, string previousScript = null)
    {
        Globals.LuaScript = new Script();
        
        // Add default Xmdl data types.
        UserData.RegisterType<XmdlDocumentHelper>();
        UserData.RegisterType<XmdlNetworkHelper>();
        UserData.RegisterType<ResponseData>();
        
        // Add default Xmdl objects.
        Globals.LuaScript.Globals["document"] = new XmdlDocumentHelper(XmdlDocument.Instance);
        Globals.LuaScript.Globals["net"] = new XmdlNetworkHelper();
        
        foreach (var (key, value) in (context?.Properties ?? new Dictionary<object, object>()))
        {
            
            Debug.Assert(key != null, nameof(key) + " != null");

            if (key is not string name)
            {
                Logger.Warning<ExecutableCodeRenderer>("Invalid name provided. Skipping...");
                continue;
            }
            
            if (name.Equals("document", StringComparison.OrdinalIgnoreCase) || name.Equals("net", StringComparison.OrdinalIgnoreCase))
            {
                Logger.Verbose<ExecutableCodeRenderer>("You are not allowed override default Xmdl objects.");
                continue;
            }

            if (!Globals.LuaScript.Globals.Get(name).IsNilOrNan())
            {
                Logger.Verbose<ExecutableCodeRenderer>("Object `{object}` already exists.", name);
                continue;
            }
            
            var type = value.GetType();
            if (type == typeof(XmdlDocumentHelper) || type == typeof(XmdlNetworkHelper) || type == typeof(ResponseData))
            {
                Logger.Verbose<ExecutableCodeRenderer>("Type `{type}` is already registered.", type);
                continue;
            }
            try
            {
                UserData.RegisterType(type);
            }
            catch (Exception e)
            {
                Logger.Warning<ExecutableCodeRenderer>("Failed to register type {type}: {message}", value.GetType(), e.Message);
            }

            try
            {
                Globals.LuaScript.Globals[name] = value;
            }
            catch (Exception e)
            {
                Logger.Warning<ExecutableCodeRenderer>("Failed to register `{name}` into the Lua Global Table: {message}", name, e.Message);
            }
        }
        List<string> errors = [];
        
        var dynValue = DynValue.Nil;
        try
        {
            dynValue = Globals.LuaScript.DoString(script, Globals.LuaScript.Globals, "xmdlua_code_" + Crc.Parse(script).ToLowerInvariant());
        }
        catch (Exception ex)
        {
            errors.Add(Markdown.ToHtml(BuildMarkdownExceptionMessage("Lua", ex, _options.IsDebugMode), _pipeline));
        }
        var result = new ExecutableCodeResult
        {
            Errors = []
            //Errors = compilationContext.Errors.Select(e => new ExecutableCodeError(e.Severity.ToString(), "Lua", e.ToString())).ToList(),
        };
        object returnValue;
        try
        {
            returnValue = dynValue?.ToObject();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            returnValue = DynValue.Nil;
            // throw;
        }
        var state = new ExecutableCodeState()
        {
            Exception = null,//scriptState.Exception,
            ReturnValue = returnValue,
            //Script = scriptState.Script,
            //Variables = scriptState.Variables,
        };
        return (state, result, errors);
    }

}
