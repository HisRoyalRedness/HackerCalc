﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisRoyalRedness.com
{
    public interface IEvaluator
    {
        IDataType Evaluate(IToken token, IConfiguration configuration);
    }

    public interface IEvaluator<TDataTypeEnum> : IEvaluator
        where TDataTypeEnum : Enum
    {
    }
}