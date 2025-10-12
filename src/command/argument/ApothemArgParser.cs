using livemap.util;
using Vintagestory.API.Common;

namespace livemap.command.argument;

public class ApothemArgParser(string argName) : IntArgParser(argName, 0, true) {
    public override string GetSyntaxExplanation(string indent) {
        return $"{indent}{GetSyntax()} {"command.arg.apothem".ToLang()}";
    }
}
