using System.Text;

namespace Bitcraft.ToolKit.CodeGeneration;

public enum LineEnding
{
    LF,
    CRLF
}

public class CodeWriter
{
    private int indentationLevel = 0;
    private readonly string indentation;
    private readonly StringBuilder sb;
    private readonly LineEnding lineEnding;

    public CodeWriter(StringBuilder sb, string indentation, LineEnding lineEnding)
    {
        if (sb == null)
            throw new ArgumentNullException(nameof(sb));

        this.sb = sb;
        this.indentation = indentation;
        this.lineEnding = lineEnding;
    }

    public CodeWriter Clone(StringBuilder sb)
    {
        return new CodeWriter(sb, indentation, lineEnding)
        {
            indentationLevel = this.indentationLevel
        };
    }

    public IDisposable Indent()
    {
        return new IndentDisposable(this);
    }

    public IDisposable SuspendIndentation()
    {
        return new IndentationChangerDisposable(this, false);
    }

    public IDisposable ResumeIndentation()
    {
        return new IndentationChangerDisposable(this, true);
    }

    public void Append(string text)
    {
        AppendIndentation();
        sb.Append(text);
    }

    public void Append(string format, params object[] args)
    {
        AppendIndentation();
        sb.AppendFormat(format, args);
    }

    public void AppendLine()
    {
        // Does not generate indentation on empty lines.

        if (lineEnding == LineEnding.LF)
            sb.Append("\n");
        else if (lineEnding == LineEnding.CRLF)
            sb.Append("\r\n");
    }

    public void AppendLine(string text)
    {
        AppendIndentation();
        sb.Append(text);
        AppendLine();
    }

    public void AppendLine(string format, params object[] args)
    {
        AppendIndentation();
        sb.AppendFormat(format, args);
        AppendLine();
    }

    private void AppendIndentation()
    {
        if (indentation.Length == 0)
            return;

        for (int level = 0; level < indentationLevel; level++)
            sb.Append(indentation);
    }

    public override string ToString()
    {
        return sb.ToString();
    }

    private class IndentDisposable : IDisposable
    {
        private readonly CodeWriter cw;

        public IndentDisposable(CodeWriter cw)
        {
            this.cw = cw;
            cw.indentationLevel++;
        }

        public void Dispose()
        {
            cw.indentationLevel--;
        }
    }

    private class IndentationChangerDisposable : IDisposable
    {
        private readonly CodeWriter cw;
        private readonly int originalIndentationLevel;

        public IndentationChangerDisposable(CodeWriter cw, bool newIndent)
        {
            this.cw = cw;
            originalIndentationLevel = cw.indentationLevel;
            cw.indentationLevel = 0;
        }

        public void Dispose()
        {
            cw.indentationLevel = originalIndentationLevel;
        }
    }
}
