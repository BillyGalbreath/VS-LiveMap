using JetBrains.Annotations;
using livemap.util;
using Vintagestory.API.Common;

namespace livemap.command.argument;

[PublicAPI]
public class ApothemArgParser : IntArgParser {
    public ApothemArgParser(string argName) : base(argName, 0, true) { }

    public override string GetSyntaxExplanation(string indent) {
        return $"{indent}{GetSyntax()} {"command.arg.apothem".ToLang()}";
    }
}
