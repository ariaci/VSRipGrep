using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace VSRipGrep.Ui
{
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
    [Guid("a7fb5a9c-9a94-4f91-abc6-04b916f250cb")]
    public class ParametersToolWindow : ToolWindowPane
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RipGrepToolWindow"/> class.
        /// </summary>
        public ParametersToolWindow() : base(null)
        {
            this.Caption = "VSRipGrep in Files";

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            this.Content = new ParametersToolWindowControl();
        }
    }
}
