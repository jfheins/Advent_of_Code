using AoCHelper;

namespace AoC_2024;

public abstract class BaseDay : BaseProblem
{
    protected override string ClassPrefix => "Day";

    public override string InputFilePath
    {
        get
        {
            var index = CalculateIndex().ToString("D2");
            return Path.Combine(InputFileDirPath, $"Day_{index}.{InputFileExtension.TrimStart('.')}");
        }
    }
}
