using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using System;
using System.Diagnostics;
using System.Threading;
using VSRipGrep.Builder;
using VSRipGrep.Models;
using VSRipGrep.Ui;

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

        internal ParametersModel Parameters { get; private set; }
        public ResultFilesModel Results { get; private set; } = new ResultFilesModel();

        internal RipGrepTask(ParametersModel parameters)
        {
            Parameters = parameters;
        }

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
            var ripGrepProcess = RipGrepProcessBuilder.Build(ripGrepTask.Parameters);

            ripGrepProcess.Start();
            while (!ripGrepProcess.StandardOutput.EndOfStream)
            {
                string resultLine = ripGrepProcess.StandardOutput.ReadLine();
                if (ripGrepTask.Token.WaitHandle.WaitOne(0))
                {
                    break;
                }

                ThreadHelper.JoinableTaskFactory.Run(async delegate
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    if (ripGrepTask.Parameters.InvertMatching)
                    {
                        var firstColon = resultLine.IndexOf(':');
                        if (firstColon >= 0)
                        {
                            resultLine = resultLine.Substring(0, firstColon);
                        }
                    }

                    ripGrepTask.Results.AddRipGrepOutput(resultLine, ripGrepTask.Parameters.Path);
                });
            }

            try
            {
                if (!ripGrepProcess.HasExited)
                {
                    ripGrepProcess.Kill();
                }
            }
            catch (Exception)
            { 
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
