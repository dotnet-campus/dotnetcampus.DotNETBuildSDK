namespace dotnetCampus.GitCommand
{
    public enum DiffType
    {
        Added,// A
        Copied,// C
        Deleted,// D
        Modified,// M
        Renamed,// R
        Changed,// T
        Unmerged,// U
        Unknown,// X
    }
}