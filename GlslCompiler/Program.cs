using System.Diagnostics;

namespace GlslCompiler
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CompileArgs? compileArgs = CompileArgs.FromFile("InputArgs.json");
            if (compileArgs is null) return;

            ClearFolders(compileArgs);

            foreach (CompiledFile file in compileArgs.FilesToCompile)
            {
                CompileFile(compileArgs, file);
            }

            Thread.Sleep(500);
        }

        private static void ClearFolders(CompileArgs args) 
        {
            foreach (CompiledFile file in args.FilesToCompile)
            {
                string destination = Path.Combine(args.OutputDir, file.Destination, $"{Path.GetFileNameWithoutExtension(file.Source)}.spv");

                string? directory = Path.GetDirectoryName(destination);
                if (directory is null) continue;

                if (Directory.Exists(directory)) Directory.Delete(directory, true);
            }
        }

        private static void CompileFile(CompileArgs args, CompiledFile file) 
        {
            string destination = Path.Combine(args.OutputDir, file.Destination, $"{Path.GetFileNameWithoutExtension(file.Source)}.spv");

            if (!Directory.Exists(Path.GetDirectoryName(destination)))
            {
                string? directory = Path.GetDirectoryName(destination);
                if (directory is not null) Directory.CreateDirectory(directory);
            }

            string arguments = $"-I. {args.SourceDir}\\{file.Source} -o {destination}";

            ProcessStartInfo startInfo = new()
            {
                FileName = args.Sdk,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using Process process = new() { StartInfo = startInfo };
            Console.WriteLine($"File: {args.SourceDir}\\{file.Source}");

            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            process.WaitForExit();

            bool canContinue = false;

            if (!string.IsNullOrEmpty(output))
            {
                Console.WriteLine("Output:");
                Console.WriteLine(output);
            }

            if (!string.IsNullOrEmpty(error))
            {
                Console.WriteLine("Error:");
                Console.WriteLine(error);
            }
            else
            {
                Console.WriteLine("Successfull");
                canContinue = true;
            }

            if (!canContinue) Console.ReadLine();

            Console.WriteLine();
        }
    }
}
