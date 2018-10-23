﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisRoyalRedness.com
{
    public enum Verbosity
    {
        ValueOnly,
        ValueAndType
    }

    public interface IDataType
    {
        object ObjectValue { get; }
        string ToString(Verbosity verbosity);
    }

    public interface IDataType<TDataTypeEnum> : IDataType
    {
        TDataTypeEnum DataType { get; }
        TNewType CastTo<TNewType>()
            where TNewType : class, IDataType<TDataTypeEnum>;
        IDataType<TDataTypeEnum> CastTo(TDataTypeEnum dataType);
    }
}