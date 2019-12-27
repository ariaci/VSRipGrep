using Microsoft.VisualStudio.Threading;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace VSRipGrep.Models
{
    class ResultFilesModel
    {
        private Dictionary<string, ResultFileModel> Files { get; set; } = new Dictionary<string, ResultFileModel>();
        public ObservableCollection<ResultFileModel> ResultFiles { get; private set; } = new ObservableCollection<ResultFileModel>();

        internal void AddRipGrepOutput(string output)
        {
            var file = output.Split('\0');
            if (file.Length != 2)
            {
                return;
            }

            ResultFileModel resultFile = null;
            if (!Files.TryGetValue(file[0], out resultFile))
            {
                resultFile = new ResultFileModel(file[0]);
                Files.Add(file[0], resultFile);
                ResultFiles.Add(resultFile);
            }

            resultFile.AddRipGrepOutput(file[1]);
        }
    }
}
