using dotnetCampus.Cli;

namespace MatrixRun
{
    internal class Options
    {
        [Option('m', nameof(Matrix))]
        public string? Matrix { get; set; }

        [Option('c', nameof(Command))]
        public string? Command { get; set; }
    }
}
