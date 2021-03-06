﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSRipGrep.Ui;

namespace VSRipGrep.Builder
{
    class RipGrepProcessBuilder
    {
        private RipGrepProcessBuilder()
        {
        }

        private static void AddArguments(ProcessStartInfo processInfo, Models.ParametersModel parameters)
        {
            processInfo.Arguments = @"""" + parameters.Pattern.Replace(@"""", @"""""") +
                @""" --no-heading --line-number --column";

            if (!parameters.UseRegularExpressions)
            {
                processInfo.Arguments += " --fixed-strings";
            }

            if (parameters.IncludeBinaryFiles)
            {
                processInfo.Arguments += " --text";
            }

            if (parameters.InvertMatching)
            {
                processInfo.Arguments += " --files-without-match";
            }

            if (parameters.MatchCase)
            {
                processInfo.Arguments += " --case-sensitive";
            }
            else
            {
                processInfo.Arguments += " --ignore-case";
            }

            if (parameters.MatchWholeWord)
            {
                processInfo.Arguments += " --word-regexp";
            }

            if (parameters.IncludeHiddenFilesOrDirectories)
            {
                processInfo.Arguments += " --hidden";
            }

            if (parameters.RespectIgnoreFiles)
            {
                processInfo.Arguments += " --no-ignore-messages";
            }
            else
            {
                processInfo.Arguments += " --no-ignore --no-ignore-global";

                var extensions = parameters.FileTypes.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var extension in extensions)
                {
                    var trimmedExtension = extension.Trim();
                    if (!string.IsNullOrEmpty(trimmedExtension))
                    {
                        processInfo.Arguments += @" --glob """ + trimmedExtension + @"""";
                    }
                }
            }
        }

        public static Process Build(Models.ParametersModel parameters)
        {
            var configuration = ToolWindowFactory.Package.GetDialogPage(typeof(Package.Configuration)) as Package.Configuration;
            if (configuration == null || parameters == null)
            {
                return null;
            }

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = configuration.RipGrepExecutable,
                    WorkingDirectory = parameters.Path,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            AddArguments(process.StartInfo, parameters);

            return process;
        }
    }
}
