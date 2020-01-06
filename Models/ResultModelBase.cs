using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSRipGrep.Models
{
    public abstract class ResultModelBase
    {
        public abstract string Text { get; }
        public abstract ResultModelBase Next { get; }
        public abstract ResultModelBase Previous { get; }
    }
}
