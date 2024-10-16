using System;
using MoonSharp.Interpreter;

namespace Markdig.Extensions.Xmdl.Lua;

[MoonSharpUserData]
public class XmdlHelperDocument(XmdlDocument document)
{
    private XmdlDocument _document = document;

    public void InsertHtml(string code)
    {
        _document.InsertHtml(code);
    }
    
    public void InsertMarkdown(string code)
    {
        _document.InsertMarkdown(code);
    }
}