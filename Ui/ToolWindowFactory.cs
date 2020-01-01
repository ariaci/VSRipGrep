using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;

namespace VSRipGrep.Ui
{
    internal sealed class ToolWindowFactory
    {
        internal static AsyncPackage Package { get; private set; }

        private ToolWindowFactory(AsyncPackage package)
        {
            Package = package;
        }

        public static void Initialize(AsyncPackage package)
        {
            Instance = new ToolWindowFactory(package);
        }

        private static ToolWindowFactory Instance
        {
            get;
            set;
        }

        private static ToolWindow ShowToolWindow<ToolWindow>() where ToolWindow : ToolWindowPane
        {
            if (Instance == null)
            {
                throw new NotSupportedException("Cannot create tool window");
            }

            return ThreadHelper.JoinableTaskFactory.Run(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                var window = Package.FindToolWindow(typeof(ToolWindow), 0, true);
                if (window == null || window.Frame == null)
                {
                    throw new NotSupportedException("Cannot create tool window");
                }

                IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
                Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());

                return window as ToolWindow;
            });
        }

        public static ParametersToolWindow ShowParametersToolWindow()
        {
            return ShowToolWindow<ParametersToolWindow>();
        }

        public static ResultsToolWindow ShowResultsToolWindow()
        {
            return ShowToolWindow<ResultsToolWindow>();
        }
    }
}
