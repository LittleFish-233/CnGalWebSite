﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CnGalWebSite.RobotClientX.Services.SensitiveWords
{
    public interface ISensitiveWordService
    {
        Task<List<string>> Check(List<string> texts);

        Task<List<string>> Check(string text);
    }
}
