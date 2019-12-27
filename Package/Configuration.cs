using System.ComponentModel;
using Microsoft.VisualStudio.Shell;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace VSRipGrep.Package
{
    class Configuration : DialogPage
    {
        [Category("General")]
        [DisplayName("VSRipGrep executable")]
        [Description("Location of VSRipGrep executable")]
        [EditorAttribute(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string RipGrepExecutable { get; set; } = "rg.exe";
    }
}
