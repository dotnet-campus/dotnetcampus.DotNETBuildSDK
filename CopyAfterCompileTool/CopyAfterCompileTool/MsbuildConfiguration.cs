using dotnetCampus.Configurations;

namespace dotnetCampus.CopyAfterCompileTool
{
    class MsbuildConfiguration : Configuration
    {
        public bool ShouldRestore
        {
            get => GetBoolean() ?? true;
            set => SetValue(value);
        }

        public int MaxCpuCount
        {
            get => GetInt32() ?? 1;
            set => SetValue(value);
        }
    }
}