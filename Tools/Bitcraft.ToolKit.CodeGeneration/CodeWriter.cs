using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitcraft.ToolKit.CodeGeneration
{
    public class CodeWriter
    {
        private bool isIndented = true;
        private int indentation;
        private StringBuilder sb;

        public CodeWriter(StringBuilder sb)
        {
            if (sb == null)
                throw new ArgumentNullException(nameof(sb));

            this.sb = sb;
        }

        public CodeWriter Clone(StringBuilder sb)
        {
            return new CodeWriter(sb)
            {
                isIndented = this.isIndented,
                indentation = this.indentation
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
            sb.Append(GetIndentation() + text);
        }

        public void Append(string format, params object[] args)
        {
            sb.Append(GetIndentation() + string.Format(format, args));
        }

        public void AppendLine()
        {
            sb.AppendLine(); // does not generate indentation on empty lines
        }

        public void AppendLine(string text)
        {
            sb.AppendLine(GetIndentation() + text);
        }

        public void AppendLine(string format, params object[] args)
        {
            sb.AppendLine(GetIndentation()  + string.Format(format, args));
        }

        private string GetIndentation()
        {
            if (isIndented)
                return new string(' ', indentation * 4);
            return string.Empty;
        }

        private class IndentDisposable : IDisposable
        {
            private CodeWriter cw;

            public IndentDisposable(CodeWriter cw)
            {
                this.cw = cw;
                cw.indentation++;
            }

            public void Dispose()
            {
                cw.indentation--;
            }
        }

        private class IndentationChangerDisposable : IDisposable
        {
            private CodeWriter cw;
            private bool originalIsIndented;

            public IndentationChangerDisposable(CodeWriter cw, bool newIndent)
            {
                this.cw = cw;
                originalIsIndented = cw.isIndented;
                cw.isIndented = newIndent;
            }

            public void Dispose()
            {
                cw.isIndented = originalIsIndented;
            }
        }
    }
}
