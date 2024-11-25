using MoonSharp.Interpreter;

namespace Markdig.Extensions.Xmdl.Lua.Helper;

[MoonSharpUserData]
public class XmdlDocumentHelper(XmdlDocument document)
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