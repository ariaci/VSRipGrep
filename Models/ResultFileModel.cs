using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace VSRipGrep.Models
{
    public class ResultFileModel : ResultModelBase, INotifyPropertyChanged
    {
        internal ResultFilesModel Files { get; private set; }

        public string File { get; private set; }
        private Dictionary<uint, ResultLineModel> Lines { get; set; } = new Dictionary<uint, ResultLineModel>();
        public ObservableCollection<ResultLineModel> ResultLines { get; private set; } = new ObservableCollection<ResultLineModel>();

        internal ResultFileModel(ResultFilesModel files, string file)
        {
            File = file;
            Files = files;
        }

        public override string Text
        {
            get
            {
                return Lines.Count > 0 ? File + " (" + Lines.Count.ToString() + ")" : File;
            }
        }

        public override ResultModelBase Next
        {
            get
            {
                var thisIndex = Files == null ? -1 : Files.ResultFiles.IndexOf(this);
                return thisIndex >= 0 && thisIndex < Files.ResultFiles.Count - 1 
                    ? Files.ResultFiles[thisIndex + 1] : null;
            }
        }

        public override ResultModelBase Previous
        {
            get
            {
                var thisIndex = Files == null ? -1 : Files.ResultFiles.IndexOf(this);
                return thisIndex > 0 ? Files.ResultFiles[thisIndex - 1] : null;
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

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Text"));
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
