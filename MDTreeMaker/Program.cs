Copyright 2020 Göksel KÜÇÜKŞAHİN

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.﻿

using System;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace MDTreeMaker
{
    // MD Tree with Links.
    class Program
    {
        static string pwd = String.Empty;
        //static string pwd = @"C:\ERU\4. Sınıf\Web_Programlama I\Vize Ödev\webProgramming-I\";
        private static readonly string regEx = @"[A-Za-z0-9_ığüşöçİĞÜŞÖÇ]+\.[A-Za-z0-9]{1,4}";
        static void Main(string[] args)
        {
            pwd = Directory.GetCurrentDirectory(); // Get Current Working Directory as String.
            if (pwd.Equals(String.Empty))
            {
                throw new DirectoryNotFoundException();
            }
            // Create Command
            var processInfo = new ProcessStartInfo("cmd.exe", "/c tree /f")
            {
                // /c is require. Some how idk
                // tree /f is "Create Directory Tree including files. f for file.
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                WorkingDirectory = pwd
            };
            StringBuilder sb = new StringBuilder(); // Create Process
            Process p = Process.Start(processInfo); // RUN COMMAND
            p.OutputDataReceived += (sender, pArgs) => sb.AppendLine(pArgs.Data);
            p.BeginOutputReadLine(); // Read the output.
            p.WaitForExit(); // Wait the Process.
            // Command done.

            // string cmdOutput = sb.ToString();       // Save output to string from String Builder.
            // string single = Filter(cmdOutput);      // Filter White_Spaces responding "special Char" And make it Single Line.
            // string border = UndoMD(single);         // Undo filter with MD specific Chars and Symbols :)
            // string output = ChangeBorders(border);  // Change borders with Double line aka 
            // output += Environment.NewLine + "_";    // For the protection.
            // string link = showMatch(output, regEx); // Linking...
            string link = ShowMatch(ChangeBorders(UndoMD(Filter(sb.ToString()))) + Environment.NewLine + "_", regEx); // Single line of upper comment
            using (var wrtr = new StreamWriter("TREE.md", false))
            {
                wrtr.Write(link);
            }
        }

        private static string ShowMatch(string text, string expr)
        {
            MatchCollection mc = Regex.Matches(text, expr);
            string newText = text;
            foreach (Match m in mc)
            {
                string[] filePaths = Directory.GetFiles(pwd, m + "", SearchOption.AllDirectories);
                foreach (string f in filePaths)
                {
                    // "[dosya_ismi](dosya_yolu/dosya_ismi)"
                    newText = newText.Replace(m + "", String.Format("[{0}]({1})", m + "", f.Replace(pwd, "").Replace("\\", "/")));
                }
            }
            return newText;
        }

        private static string Filter(string cmd)
        {
            // Ʃ = New_Line
            // ƛ = 4 spaces
            // Ɛ = 3 spaces
            // Ω = 2 spaces
            // Φ = 1 spaces
            return cmd.Replace(Environment.NewLine, "Ʃ").Replace("    ", "ƛ").Replace("   ", "Ɛ").Replace("  ", "Ω").Replace(" ", "Φ");
        }

        private static string ChangeBorders(string borders)
        {
            // └ = ╚
            // ─ = ═
            // ├ = ╠
            // │ = ║
            return borders.Replace("└", "╚").Replace("─", "═").Replace("├", "╠").Replace("│", "║"); ;
        }

        private static string Undo(string str)
        {
            // \n = Ʃ
            // 4 spaces = ƛ
            // 3 spaces = Ɛ
            // 2 spaces = Ω
            // 1 spaces = Φ
            return str.Replace("Ʃ", "\n").Replace("ƛ", "    ").Replace("Ɛ", "   ").Replace("Ω", "  ").Replace("Φ", "  ");
        }

        private static string UndoMD(string str)
        {
            // Ʃ = \\n = (\n in output)
            // ƛ = 10 x &nbsp;
            // Ɛ =  8 x &nbsp;
            // Ω =  5 x &nbsp;
            // Φ =  2 x &nbsp;
            return str.Replace("Ʃ", "\\\n").Replace("ƛ", "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;").Replace("Ɛ", "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;").Replace("Ω", "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;").Replace("Φ", "&nbsp;&nbsp;");
        }
    }
}
