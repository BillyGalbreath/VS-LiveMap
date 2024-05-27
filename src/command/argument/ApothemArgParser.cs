using JetBrains.Annotations;
using Vintagestory.API.Common;

namespace livemap.command.argument;

[PublicAPI]
public class ApothemArgParser : IntArgParser {
    public ApothemArgParser(string argName, int max) : base(argName, 0, max, 0, true) { }

    public override string GetSyntaxExplanation(string indent) {
        return indent + GetSyntax() + " the apothem range to render";
    }
}
