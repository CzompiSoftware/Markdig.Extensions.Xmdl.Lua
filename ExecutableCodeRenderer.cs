using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using CzomPack.Network;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;
using Markdig.Extensions.Xmdl.ExecutableCode;
using Markdig.Extensions.Xmdl.Lua.Helper;
using MoonSharp.Interpreter;

namespace Markdig.Extensions.Xmdl.Lua;

internal class ExecutableCodeRenderer(IExecutableCodeOptions options, MarkdownPipeline pipeline) : ExecutableCode.ExecutableCodeRenderer(options, pipeline)
{
    private readonly IExecutableCodeOptions _options = options;
    private readonly MarkdownPipeline _pipeline = pipeline;

    public override async Task<(ExecutableCodeState, ExecutableCodeResult, List<string>)> ExecuteAsync(string script, string previousScript = null)
    {
        Globals.LuaScript = new Script();
        UserData.RegisterType<XmdlDocumentHelper>();
        UserData.RegisterType<XmdlNetworkHelper>();
        UserData.RegisterType<ResponseData>();
        // Table globalContext = new(Globals.LuaScript);
        var document = new Table(Globals.LuaScript);
        // document["insertHtml"] = "HTML";
        // document["insertMarkdown"] = "MD";
        // Globals.LuaScript.Globals["document"] = document;
        Globals.LuaScript.Globals["document"] = new XmdlDocumentHelper(XmdlDocument.Instance);
        Globals.LuaScript.Globals["net"] = new XmdlNetworkHelper();
        //globalContext.Set("XmdlDoc", DynValue.FromObject(Globals.LuaScript, XmdlDocument.Instance));
        // Globals.LuaScript.LoadFunction(script);
        List<string> errors = new();
        
        DynValue dynValue = DynValue.Nil;
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
            Errors = new()
            //Errors = compilationContext.Errors.Select(e => new ExecutableCodeError(e.Severity.ToString(), "Lua", e.ToString())).ToList(),
        };
        //object returnValue = null;
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
