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
                return File + " (" + Lines.Count.ToString() + ")";
            }
        }

        internal void AddRipGrepOutput(string output)
        {
            var line = output.Split(":".ToCharArray(), 2);
            if (line.Length != 2)
            {
                return;
            }

            ResultLineModel resultLine = null;
            var lineNumber = Convert.ToUInt32(line[0]);
            if (!Lines.TryGetValue(lineNumber, out resultLine))
            {
                resultLine = new ResultLineModel(this, lineNumber, line[1]);
                Lines.Add(lineNumber, resultLine);
                ResultLines.Add(resultLine);

                PropertyChanged(this, new PropertyChangedEventArgs("Text"));
            }
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
