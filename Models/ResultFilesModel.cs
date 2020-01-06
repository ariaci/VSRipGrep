using Microsoft.VisualStudio.Threading;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace VSRipGrep.Models
{
    public class ResultFilesModel : ResultModelBase
    {
        internal string BasePath { get; private set; }
        private Dictionary<string, ResultFileModel> Files { get; set; } = new Dictionary<string, ResultFileModel>();
        public ObservableCollection<ResultFileModel> ResultFiles { get; private set; } = new ObservableCollection<ResultFileModel>();

        internal ResultFilesModel(string basePath)
        {
            BasePath = basePath;
        }

        public override string Text 
        { 
            get
            {
                return BasePath;
            }
        }

        public override ResultModelBase Next
        {
            get
            {
                return null;
            }
        }

        public override ResultModelBase Previous
        {
            get
            {
                return null;
            }
        }

        internal void AddRipGrepOutput(string output)
        {
            var file = output.Split(":".ToCharArray(), 2);
            if (file.Length != 1 && file.Length != 2)
            {
                return;
            }

            file[0] = System.IO.Path.Combine(BasePath, file[0]);

            ResultFileModel resultFile = null;
            if (!Files.TryGetValue(file[0], out resultFile))
            {
                resultFile = new ResultFileModel(this, file[0]);
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
