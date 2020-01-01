using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace VSRipGrep.Models
{
    public class ResultLineModel
    {
        internal uint Line { get; private set; } = 0;
        internal uint Column{ get; private set; } = 0;
        internal string Content { get; private set; }
        internal ResultFileModel File { get; private set; } = null;

        internal ResultLineModel(ResultFileModel file, uint line, uint column, string content)
        {
            File = file;
            Line = line;
            Column = column;
            Content = content;
        }

        public string Text
        {
            get
            {
                return Line.ToString() + ", " + Column.ToString()+ ": " + Content;
            }
        }

        internal void GotoLocation()
        {
            ThreadHelper.JoinableTaskFactory.Run(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                var editorWindow = File?.GotoFile();
                var textSelection = editorWindow?.Document.Selection as TextSelection;

                textSelection?.MoveToLineAndOffset((int)Line, (int)Column);
            });
        }
    }
}