using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using System;
using System.Threading;
using VSRipGrep.Models;

namespace VSRipGrep.Tasks
{
    class RipGrepTask
    {
        private CancellationTokenSource TokenSource { get; } = new CancellationTokenSource();
        private CancellationToken Token 
        { 
            get
            {
                return TokenSource.Token;
            }
        }
        private JoinableTask Task { get; set; }

        public ResultFilesModel Results { get; private set; } = new ResultFilesModel();

        public void Run()
        {
            Task = ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await System.Threading.Tasks.Task.Run(delegate {
                    ExecuteRipGrep(this);
                });
            });
        }

        static private void ExecuteRipGrep(RipGrepTask ripGrepTask)
        {
            string[] testValues = {
                    "D:\\TEMP\\Test\\App.xaml.cs\013:            DispatcherHelper.Initialize();",
                    "D:\\TEMP\\Test\\MainWindow.xaml.cs\012:        /// Initializes a new instance of the MainWindow class.",
                    "D:\\TEMP\\Test\\MainWindow.xaml.cs\016:            InitializeComponent();"
                };

            foreach (var value in testValues)
            {
                if (ripGrepTask.Token.WaitHandle.WaitOne(TimeSpan.FromSeconds(5)))
                {
                    break;
                }

                ThreadHelper.JoinableTaskFactory.Run(async delegate
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    ripGrepTask.Results.AddRipGrepOutput(value);
                });
            }

            ripGrepTask.Task = null;
        }

        public void Cancel()
        {
            if (Task == null)
            {
                return;
            }

            var waitTask = Task;

            TokenSource.Cancel();
            waitTask.Join();
        }
    }
}
