using System;

namespace Prototype.Data
{
    [Serializable]
    public class BaseData
    {
        public IdData IdMeta;
        public IdData IdUser;
        public IdData IdSession;
        public string ResourceTag;
    }
}
