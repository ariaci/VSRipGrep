using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace VSRipGrep.Models
{
    public class ResultFileModel : INotifyPropertyChanged
    {
        public string File { get; private set; }
        private Dictionary<uint, ResultLineModel> Lines { get; set; } = new Dictionary<uint, ResultLineModel>();
        public ObservableCollection<ResultLineModel> ResultLines { get; private set; } = new ObservableCollection<ResultLineModel>();

        internal ResultFileModel(string file)
        {
            File = file;
        }

        public string Text
        {
            get
            {
                return Lines.Count > 0 ? File + " (" + Lines.Count.ToString() + ")" : File;
            }
        }

        internal void AddRipGrepOutput(string output)
        {
            var splittedOutput = output.Split(":".ToCharArray(), 3);
            if (splittedOutput.Length != 2 && splittedOutput.Length != 3)
            {
                return;
            }

            ResultLineModel resultLine;

            var line = Convert.ToUInt32(splittedOutput[0]);
            var column = Convert.ToUInt32(splittedOutput[1]);
            if (!Lines.TryGetValue(line, out resultLine))
            {
                resultLine = new ResultLineModel(this, line, column, splittedOutput[2]);
                Lines.Add(line, resultLine);
                ResultLines.Add(resultLine);

                PropertyChanged(this, new PropertyChangedEventArgs("Text"));
            }
        }

        internal Window GotoFile()
        {
            if (System.IO.File.Exists(File) == false)
            {
                return null;
            }

            return ThreadHelper.JoinableTaskFactory.Run(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                var dte = Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(DTE)) as DTE;
                return dte?.ItemOperations.OpenFile(File);
            });
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
