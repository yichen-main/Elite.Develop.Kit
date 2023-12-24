namespace Elite.IIoT.Core.Architects.Configures;
internal sealed class YamlSource : FileConfigurationSource
{
    public override IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        EnsureDefaults(builder);
        return new YamlProvider(this);
    }
    sealed class YamlProvider(FileConfigurationSource source) : FileConfigurationProvider(source)
    {
        public override void Load(Stream stream)
        {
            YamlStream yamlStream = [];
            yamlStream.Load(new StreamReader(stream));
            SortedDictionary<string, string?> data = new(StringComparer.Ordinal);
            if (yamlStream.Documents.Count > 0) VisitYamlNode(string.Empty, yamlStream.Documents[default].RootNode);
            void VisitYamlNode(string context, YamlNode node)
            {
                string currentPath;
                Stack<string> stackContext = new();
                if (node is YamlScalarNode scalarNode)
                {
                    EnterContext(context);
                    data[currentPath] = scalarNode.Value ?? string.Empty;
                    ExitContext();
                }
                else if (node is YamlMappingNode mappingNode)
                {
                    EnterContext(context);
                    foreach (KeyValuePair<YamlNode, YamlNode> yamlNode in mappingNode.Children)
                    {
                        context = ((YamlScalarNode)yamlNode.Key).Value ?? string.Empty;
                        VisitYamlNode(context, yamlNode.Value);
                    }
                    ExitContext();
                }
                else if (node is YamlSequenceNode sequenceNode)
                {
                    EnterContext(context);
                    for (int item = default; item < sequenceNode.Children.Count; item++)
                        VisitYamlNode(item.ToString(CultureInfo.CurrentCulture), sequenceNode.Children[item]);
                    ExitContext();
                }
                void EnterContext(string context)
                {
                    if (!string.IsNullOrEmpty(context)) stackContext.Push(context);
                    currentPath = ConfigurationPath.Combine(stackContext.Reverse());
                }
                void ExitContext()
                {
                    if (stackContext.Count is not 0) stackContext.Pop();
                    currentPath = ConfigurationPath.Combine(stackContext.Reverse());
                }
            }
            Data = data;
        }
    }
}