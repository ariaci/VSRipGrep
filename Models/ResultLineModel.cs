using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace VSRipGrep.Models
{
    public class ResultLineModel
    {
        internal uint Line { get; private set; } = 0;
        internal string Content { get; private set; }
        internal ResultFileModel File { get; private set; } = null;

        internal ResultLineModel(ResultFileModel file, uint line, string content)
        {
            File = file;
            Line = line;
            Content = content;
        }

        public string Text
        {
            get
            {
                return Line.ToString() + ": " + Content;
            }
        }

        internal void GotoLocation()
        {
            if (System.IO.File.Exists(File.File) == false)
            {
                return;
            }

            ThreadHelper.JoinableTaskFactory.Run(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                var dte = Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(DTE)) as DTE;
                var editorWindow = dte?.ItemOperations.OpenFile(File.File);
                var textSelection = editorWindow?.Document.Selection as TextSelection;

                textSelection?.GotoLine((int)Line);
            });
        }
    }
}