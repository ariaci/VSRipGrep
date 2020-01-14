using System;
using System.ComponentModel;

namespace VSRipGrep.Models
{
    internal class ParametersModel : INotifyPropertyChanged
    {
        public ParametersModel Clone()
        {
            return MemberwiseClone() as ParametersModel;
        }

        private void propertySetter<T>(ref T modelValue, T value, string propertyName) where T: IComparable
        {
            if (value.CompareTo(modelValue) == 0)
            {
                return;
            }

            modelValue = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string m_path = "C:\\";
        public string Path
        {
            get
            {
                return m_path;
            }
            set
            {
                propertySetter(ref m_path, value, "Path");
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsValidPath"));
            }
        }

        public bool IsValidPath
        {
            get
            {
                return System.IO.Directory.Exists(Path);
            }
        }

        public string Pattern { get; set; } = "";
        public bool MatchCase { get; set; } = true;
        public bool MatchWholeWord { get; set; } = false;
        public bool UseRegularExpressions { get; set; } = false;
        public bool InvertMatching { get; set; } = false;
        public bool IncludeBinaryFiles { get; set; } = false;
        public bool IncludeHiddenFilesOrDirectories { get; set; } = false;
        public bool RespectIgnoreFiles { get; set; } = false;
        public string FileTypes { get; set; } = "";

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
