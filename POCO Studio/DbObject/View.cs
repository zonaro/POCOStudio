using System;

namespace Db.DbObject
{
    public class View : Table
    {
        public override POCODbType DbType { get { return POCODbType.View; } }
    }
}
