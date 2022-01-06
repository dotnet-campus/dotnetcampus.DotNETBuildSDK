using System.Xml.Serialization;
using dotnetCampus.Configurations;

namespace dotnetCampus.CopyAfterCompileTool
{
  public  class MsbuildConfiguration : Configuration
    {
        [XmlElement]
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