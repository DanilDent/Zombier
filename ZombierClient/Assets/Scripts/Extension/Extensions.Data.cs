using Prototype.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Prototype.Extensions
{
    public static class ExtensionsData
    {
        public static IdData GetIdUser(this IdData target)
        {
            return new IdData(target.Value);
        }

        public static IdData GetIdSession(this IdData target)
        {
            return new IdData(target.Value);
        }

        public static void RenameKeys<TValue>(this IDictionary<IdData, TValue> dic, Func<IdData, IdData> renameFunc)
        {
            var keys = dic.Keys.ToList();
            foreach (var key in keys)
            {
                TValue value = dic[key];
                dic.Remove(key);
                dic.Add(renameFunc(key), value);
            }
        }
    }

}