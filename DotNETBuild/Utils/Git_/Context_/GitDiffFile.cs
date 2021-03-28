using System.IO;

namespace dotnetCampus.GitCommand
{
    public class GitDiffFile
    {
        /// <inheritdoc />
        public GitDiffFile(DiffType diffType, FileInfo file)
        {
            DiffType = diffType;
            File = file;
        }

        public DiffType DiffType { get; }
        public FileInfo File { get; }
    }
}