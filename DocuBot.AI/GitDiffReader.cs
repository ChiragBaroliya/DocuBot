using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DocuBot.AI
{
    public static class GitDiffReader
    {
        public static List<string> GetChangedFiles()
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "git",
                        Arguments = "diff --cached --name-only",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                var output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    return new List<string>();
                }

                var files = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                return new List<string>(files);
            }
            catch
            {
                return new List<string>();
            }
        }
    }
}
