using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AoCHelper;

namespace AoC_2021
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
