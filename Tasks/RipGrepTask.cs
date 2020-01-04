using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using System;
using System.Threading;
using VSRipGrep.Builder;
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

        public event EventHandler TaskChanged;

        private JoinableTask m_task;
        private JoinableTask Task
        {
            get
            {
                return m_task;
            }
            set
            {
                if (m_task == value)
                {
                    return;
                }

                m_task = value;
                TaskChanged?.Invoke(this, null);
            }
        }

        internal ParametersModel Parameters { get; private set; }
        internal ResultFilesModel Results { get; private set; } = new ResultFilesModel();

        internal RipGrepTask(ParametersModel parameters)
        {
            Parameters = parameters;
        }

        internal void Run()
        {
            Task = ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await System.Threading.Tasks.Task.Run(delegate {
                    ExecuteRipGrep(this);
                });
            });
        }

        internal bool IsRunning
        {
            get
            {
                return Task != null;
            }
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

        internal void Cancel()
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
