namespace VSRipGrep.Ui
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.OLE.Interop;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;

    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    /// <remarks>
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    /// usually implemented by the package implementer.
    /// <para>
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its
    /// implementation of the IVsUIElementPane interface.
    /// </para>
    /// </remarks>
    [Guid("8c54d42e-bfe0-4fc1-8c10-2284e5db6756")]
    public class ResultsToolWindow : ToolWindowPane, IOleCommandTarget
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResultsToolWindow"/> class.
        /// </summary>
        public ResultsToolWindow() : base(null)
        {
            this.Caption = "RipGrep Find Results";

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            this.Content = new ResultsToolWindowControl();
        }

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            if (pguidCmdGroup != VSConstants.CMDSETID.StandardCommandSet97_guid)
            {
                return VSConstants.S_OK;
            }

            switch (nCmdID)
            {
                case (uint)VSConstants.VSStd97CmdID.NextLocation:
                case (uint)VSConstants.VSStd97CmdID.PreviousLocation:
                    (Content as ResultsToolWindowControl)?.ExecuteCommand((VSConstants.VSStd97CmdID)nCmdID);
                    break;
            }

            return VSConstants.S_OK;
        }

        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            if (pguidCmdGroup != VSConstants.CMDSETID.StandardCommandSet97_guid)
            {
                return VSConstants.S_OK;
            }

            switch (prgCmds[0].cmdID)
            {
                case (uint)VSConstants.VSStd97CmdID.NextLocation:
                case (uint)VSConstants.VSStd97CmdID.PreviousLocation:
                    prgCmds[0].cmdf = (uint)(Content as ResultsToolWindowControl)?.QueryCommandStatus((VSConstants.VSStd97CmdID)prgCmds[0].cmdID);
                    break;
            }

            return VSConstants.S_OK;
        }

        public override void OnToolWindowCreated()
        {
            base.OnToolWindowCreated();

            ThreadHelper.JoinableTaskFactory.Run(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                var trackSelection = GetService(typeof(IVsTrackSelectionEx)) as IVsTrackSelectionEx;
                trackSelection.OnElementValueChange((int)VSConstants.VSSELELEMID.SEID_ResultList, 0, this);
            });
        }
    }
}
