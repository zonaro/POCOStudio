using System;

namespace Db.DbObject
{
    public class Function : Procedure
    {
        public override POCODbType DbType { get { return POCODbType.Function; } }
    }
}
