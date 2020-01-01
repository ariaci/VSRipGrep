using Microsoft.VisualStudio.Threading;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace VSRipGrep.Models
{
    class ResultFilesModel
    {
        private Dictionary<string, ResultFileModel> Files { get; set; } = new Dictionary<string, ResultFileModel>();
        public ObservableCollection<ResultFileModel> ResultFiles { get; private set; } = new ObservableCollection<ResultFileModel>();

        internal void AddRipGrepOutput(string output, string basePath)
        {
            var file = output.Split(":".ToCharArray(), 2);
            if (file.Length != 1 && file.Length != 2)
            {
                return;
            }

            file[0] = System.IO.Path.Combine(basePath, file[0]);

            ResultFileModel resultFile = null;
            if (!Files.TryGetValue(file[0], out resultFile))
            {
                resultFile = new ResultFileModel(file[0]);
                Files.Add(file[0], resultFile);
                ResultFiles.Add(resultFile);
            }

            if (file.Length == 2)
            {
                resultFile.AddRipGrepOutput(file[1]);
            }
        }
    }
}
