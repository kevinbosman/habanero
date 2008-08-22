using System.Collections.Generic;
using Habanero.Base.Exceptions;

namespace Habanero.Base
{
    public class SourceDB : Source
    {
        private Source _source;

        public SourceDB(Source source)
            : base(source.Name, source.EntityName)
        {
            _source = source;
        }

        public override string Name
        {
            get { return _source.Name; }
            set { _source.Name = value; }
        }

        public override string EntityName
        {
            get { return _source.EntityName; }
            set { _source.EntityName = value; }
        }

        public override JoinList Joins
        {
            get { return _source.Joins; }
        }

        public string CreateSQL()
        {
            return CreateSQL(new SqlFormatter("", ""));
        }

        private string GetJoinString(Source source, SqlFormatter sqlFormatter)
        {
            string joinString = "";
            foreach (Join join in source.Joins)
            {
                joinString += " " + GetJoinString(join, sqlFormatter);
            }
            return joinString;
        }

        private string GetJoinString(Join join, SqlFormatter sqlFormatter)
        {
            if (join.JoinFields.Count == 0)
            {
                string message = string.Format("SQL cannot be created for the source '{0}' because it has a join to '{1}' without join fields",
                                               Name, join.ToSource.Name);
                throw new HabaneroDeveloperException(message, "Please check how you are building your join clause structure.");
            }
            Source.Join.JoinField joinField = join.JoinFields[0];
            string joinString = string.Format("JOIN {0} ON {1}.{2} = {0}.{3}",
                                              sqlFormatter.DelimitTable(join.ToSource.EntityName), sqlFormatter.DelimitTable(join.FromSource.EntityName),
                                              sqlFormatter.DelimitField(joinField.FromField.FieldName), sqlFormatter.DelimitField(joinField.ToField.FieldName));
            if (join.JoinFields.Count > 1)
            {
                for (int i = 1; i < join.JoinFields.Count; i++)
                {
                    joinField = join.JoinFields[i];
                    joinString += string.Format(" AND {0}.{2} = {1}.{3}", 
                        sqlFormatter.DelimitTable(join.FromSource.EntityName),  sqlFormatter.DelimitTable(join.ToSource.EntityName)    ,
                        sqlFormatter.DelimitField(joinField.FromField.FieldName), sqlFormatter.DelimitField(joinField.ToField.FieldName));
                }
                
            }
            if (join.ToSource.Joins.Count > 0)
            {
                joinString += GetJoinString(join.ToSource, sqlFormatter);
            }
            return joinString;
        }

        public string CreateSQL(SqlFormatter sqlFormatter)
        {
            if (Joins.Count == 0) return sqlFormatter.DelimitTable(EntityName);
            string joinString = GetJoinString(this, sqlFormatter);
            return sqlFormatter.DelimitTable(this.EntityName) + joinString;
        }
    }
}