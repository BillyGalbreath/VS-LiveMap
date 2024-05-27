using JetBrains.Annotations;
using Vintagestory.API.Common;

namespace livemap.command.argument;

[PublicAPI]
public class CenterPositionArgParser : WorldPosition2DArgParser {
    public CenterPositionArgParser(string argName, ICoreAPI api, bool isMandatoryArg) : base(argName, api, isMandatoryArg) { }

    public override string GetSyntaxExplanation(string indent) {
        return indent + GetSyntax() + " the center position";
    }
}
