using AoCHelper;

namespace AoC_2022
{
    public abstract class BaseDay : BaseProblem
    {
        protected override string ClassPrefix { get; } = "Day";

        public override string InputFilePath
        {
            get
            {
                var index = CalculateIndex().ToString("D2");
                return Path.Combine(InputFileDirPath, $"Day_{index}.{InputFileExtension.TrimStart('.')}");
            }
        }
    }
}
