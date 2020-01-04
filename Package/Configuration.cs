using System.ComponentModel;
using Microsoft.VisualStudio.Shell;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace VSRipGrep.Package
{
    class Configuration : DialogPage, INotifyPropertyChanged
    {
        private string m_ripGrepExecutable = "rg.exe";
        [Category("General")]
        [DisplayName("VSRipGrep executable")]
        [Description("Location of VSRipGrep executable")]
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string RipGrepExecutable
        {
            get
            {
                return m_ripGrepExecutable;
            }
            set
            {
                if (m_ripGrepExecutable == value)
                {
                    return;
                }

                m_ripGrepExecutable = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RipGrepExecutable"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
