using livemap.util;
using Vintagestory.API.Common;

namespace livemap.command.argument;

public class CenterPositionArgParser(string argName, ICoreAPI api) : WorldPosition2DArgParser(argName, api, false) {
    public override string GetSyntaxExplanation(string indent) {
        return $"{indent}{GetSyntax()} {"command.arg.center".ToLang()}";
    }
}
