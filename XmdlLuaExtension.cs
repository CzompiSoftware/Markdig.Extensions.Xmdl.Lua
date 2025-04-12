using Markdig.Extensions.Xmdl.ExecutableCode;
using Markdig.Renderers;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Markdig.Extensions.Xmdl.Lua;

public class XmdlLuaExtension : XmdlBaseExtension
{
    public XmdlLuaExtension() : base() { }

    public XmdlLuaExtension(ExecutableCodeOptions options, Uri currentUri = null) : base(options, currentUri)
    {
    }

    public override void Setup(MarkdownPipelineBuilder pipeline)
    {
        base.Setup(pipeline);
        pipeline.InlineParsers.ReplaceOrAdd<ExecutableCodeInlineParser>(new ExecutableCodeInlineParser("lua", "Lua"));
        pipeline.BlockParsers.ReplaceOrAdd<ExecutableCodeBlockParser>(new ExecutableCodeBlockParser("lua", "Lua"));
    }
    public override void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
        base.Setup(pipeline, renderer);
        var codeRenderer = new ExecutableCodeRenderer(_options, pipeline);
        renderer.ObjectRenderers.ReplaceOrAdd<ExecutableCodeInlineRenderer>(new ExecutableCodeInlineRenderer(codeRenderer, _options, pipeline));
        renderer.ObjectRenderers.ReplaceOrAdd<ExecutableCodeBlockRenderer>(new ExecutableCodeBlockRenderer(codeRenderer, _options, pipeline));
    }

}
