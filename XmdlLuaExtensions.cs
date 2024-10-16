using Markdig.Extensions.Xmdl;
using Markdig.Extensions.Xmdl.Lua;
using System;

namespace Markdig.Extensions.Xmdl;

public static class XmdlLuaExtensions
{
    public static MarkdownPipelineBuilder UseXmdlLua(this MarkdownPipelineBuilder pipeline, ExecutableCodeOptions options, Uri currentUri)
    {
        pipeline = pipeline.UseSimpleXmdl(currentUri);
        pipeline.Extensions.Add(new XmdlLuaExtension(options, currentUri));
        return pipeline;
    }
}
