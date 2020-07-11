using System.Collections.Generic;

using dotnetCampus.Cli;

namespace dotnetCampus.MatrixRun
{
    internal class Options
    {
        [Option('m', nameof(Matrix))]
        public IReadOnlyDictionary<string, string>? Matrix { get; set; }

        [Option('c', nameof(Command))]
        public string? Command { get; set; }
    }
}
