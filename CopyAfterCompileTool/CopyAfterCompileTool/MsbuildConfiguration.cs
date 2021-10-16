using dotnetCampus.Configurations;

namespace dotnetCampus.CopyAfterCompileTool
{
    class MsbuildConfiguration : Configuration
    {
        public bool ShouldRestore
        {
            set => SetValue(value);
            get => GetBoolean() ?? true;
        }

        public bool ShouldParallel
        {
            set => SetValue(value);
            get => GetBoolean() ?? true;
        }
    }
}