using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSRipGrep.Models;

namespace VSRipGrep.Tools
{
    internal sealed class ResultModelHelper
    {
        static internal ResultFilesModel Root(ResultModelBase current)
        {
            if (current == null)
            {
                return null;
            }

            var files = current as ResultFilesModel;
            if (files != null)
            {
                return files;
            }

            var file = current as ResultFileModel;
            if (file != null)
            {
                return file.Files;
            }

            return (current as ResultLineModel)?.File?.Files;
        }

        static internal ResultModelBase Next(ResultModelBase current)
        {
            var root = Root(current);
            if (root == null || root.ResultFiles.Count == 0)
            {
                return null;
            }

            var file = current as ResultFileModel;
            if (file != null && file.ResultLines.Count > 0)
            {
                return file.ResultLines[0];
            }

            ResultModelBase next = current == root ? root.ResultFiles[0] : current.Next;
            if (next == null && current is ResultLineModel)
            {
                next = (current as ResultLineModel).File.Next;
            }

            var nextFile = next as ResultFileModel;
            return nextFile != null && nextFile.ResultLines.Count > 0 ? nextFile.ResultLines[0] : next;
        }

        static internal ResultModelBase Previous(ResultModelBase current)
        {
            var root = Root(current);
            if (root == null || root.ResultFiles.Count == 0)
            {
                return null;
            }

            var previous = current == root ? root.ResultFiles[root.ResultFiles.Count - 1] : current.Previous;
            if (previous == null && current is ResultLineModel)
            {
                previous = (current as ResultLineModel).File.Previous;
            }

            var previousFile = previous as ResultFileModel;
            return previousFile != null && previousFile.ResultLines.Count > 0 
                ? previousFile.ResultLines[previousFile.ResultLines.Count - 1] : previous;
        }

        static internal bool Edit(ResultModelBase model)
        {
            if (model == null)
            {
                return false;
            }

            var fileModel = model as ResultFileModel;
            var lineModel = model as ResultLineModel;

            if (fileModel != null)
            {
                fileModel.GotoFile();
                return true;
            }
            else if (lineModel != null)
            {
                lineModel.GotoLocation();
                return true;
            }

            return false;
        }
    }
}
