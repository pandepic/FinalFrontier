using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalFrontier.Database.Tables
{
    public abstract class Table
    {
        public abstract bool Insert(SQLiteCommand command);
        public abstract bool Update(SQLiteCommand command);
    }
}
