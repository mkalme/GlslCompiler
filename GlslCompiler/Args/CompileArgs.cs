using System.Text.Json.Nodes;

namespace GlslCompiler
{
    public class CompileArgs
    {
        public string Sdk { get; set; } = string.Empty;
        public string SourceDir { get; set; } = string.Empty;
        public string OutputDir { get; set; } = string.Empty;
        public bool MakeLowercase { get; set; } = true;
        public IList<CompiledFile> FilesToCompile { get; set; } = [];

        public static CompileArgs? FromFile(string path) 
        {
            JsonNode? fileNode = JsonNode.Parse(File.ReadAllText(path));
            if (fileNode is null || fileNode is not JsonObject obj) return null;

            if (!obj.TryGetPropertyValue("Sdk", out JsonNode? sdkNode) || sdkNode is null) return null;
            string sdk = sdkNode.GetValue<string>();

            if (!obj.TryGetPropertyValue("SourceDir", out JsonNode? sourceDirNode) || sourceDirNode is null) return null;
            string sourceDir = sourceDirNode.GetValue<string>();

            if (!obj.TryGetPropertyValue("OutputDir", out JsonNode? outputDirNode) || outputDirNode is null) return null;
            string outputDir = outputDirNode.GetValue<string>();

            if (!obj.TryGetPropertyValue("MakeLowercase", out JsonNode? makeLowercaseNode) || makeLowercaseNode is null) return null;
            bool makeLowercase = makeLowercaseNode.GetValue<bool>();

            JsonNode? arrayNode = obj["FilesToCompile"];
            if (arrayNode is null || arrayNode is not JsonArray array) return null;

            IList<CompiledFile> files = [];
            foreach (JsonNode? file in array)
            {
                if (file is null || file is not JsonArray fileArray || fileArray.Count != 2) continue;

                CompiledFile compiledFile = new()
                {
                    Source = fileArray[0]?.GetValue<string>() ?? string.Empty,
                    Destination = fileArray[1]?.GetValue<string>() ?? string.Empty
                };

                files.Add(compiledFile);
            }

            return new CompileArgs() 
            {
                Sdk = sdk,
                SourceDir = sourceDir,
                OutputDir = outputDir,
                MakeLowercase = makeLowercase,
                FilesToCompile = files
            };
        }
    }
}
