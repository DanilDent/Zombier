﻿using Prototype.Data;
using System;

namespace Prototype.Service
{
    public class IdProviderService
    {
        public static IdData GetNewId()
        {
            return new IdData { Value = Guid.NewGuid().ToString() };
        }
    }
}
