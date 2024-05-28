using JetBrains.Annotations;
using livemap.util;
using Vintagestory.API.Common;

namespace livemap.command.argument;

[PublicAPI]
public class CenterPositionArgParser : WorldPosition2DArgParser {
    public CenterPositionArgParser(string argName, ICoreAPI api) : base(argName, api, false) { }

    public override string GetSyntaxExplanation(string indent) {
        return $"{indent}{GetSyntax()} {"command.arg.center".ToLang()}";
    }
}
