//---------------------------------------------------------------------------------
// Copyright (C) 2007 Chillisoft Solutions
// 
// This file is part of Habanero Standard.
// 
//     Habanero Standard is free software: you can redistribute it and/or modify
//     it under the terms of the GNU Lesser General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     Habanero Standard is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU Lesser General Public License for more details.
// 
//     You should have received a copy of the GNU Lesser General Public License
//     along with Habanero Standard.  If not, see <http://www.gnu.org/licenses/>.
//---------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using Habanero.BO.ClassDefinition;
using Habanero.Base;

namespace Habanero.BO.SqlGeneration
{
    /// <summary>
    /// Generates "select" sql statements to read a specified business
    /// object's properties from the database
    /// </summary>
    public class SelectStatementGenerator
    {
        private readonly IDatabaseConnection _connection;
        private BusinessObject _bo;
        private ClassDef _classDef;

        /// <summary>
        /// Constructor to initialise the generator
        /// </summary>
        /// <param name="bo">The business object whose properties are to
        /// be read</param>
        /// <param name="classDef">The class definition</param>
        /// <param name="connection">A database connection</param>
        public SelectStatementGenerator(BusinessObject bo, ClassDef classDef, IDatabaseConnection connection)
        {
            _bo = bo;
            _classDef = classDef;
            _connection = connection;
        }

        /// <summary>
        /// Constructor to initialise the generator
        /// </summary>
        /// <param name="bo">The business object whose properties are to
        /// be read</param>
        /// <param name="connection">A database connection</param>
        public SelectStatementGenerator(BusinessObject bo, IDatabaseConnection connection)
            : this(bo, bo.ClassDef, connection)
        {
        }

        /// <summary>
        /// Generates a sql statement to read the business
        /// object's properties from the database
        /// </summary>
        /// <param name="limit">The limit</param>
        /// <returns>Returns a string</returns>
        public string Generate(int limit)
        {
            IList classDefs = new ArrayList();
            ClassDef currentClassDef = _classDef;
            while (currentClassDef != null)
            {
                classDefs.Add(currentClassDef);
                currentClassDef = currentClassDef.SuperClassClassDef;
            }

            string statement = "SELECT ";
            if (limit > 0)
            {
                statement += " " + _connection.GetLimitClauseForBeginning(limit) + " ";
            }

            foreach (BOProp prop in _bo.Props.SortedValues)
            {
                string tableName = GetTableName(prop, classDefs);
                statement += tableName + ".";
                statement += _connection.LeftFieldDelimiter;
                statement += prop.DatabaseFieldName;
                statement += _connection.RightFieldDelimiter;
                statement += ", ";
            }

            statement = statement.Remove(statement.Length - 2, 2);
            currentClassDef = _classDef;
            while (currentClassDef.IsUsingSingleTableInheritance())
            {
                currentClassDef = currentClassDef.SuperClassClassDef;
            }
            statement += " FROM " + currentClassDef.TableName;
            string where = " WHERE ";

            while (currentClassDef.IsUsingClassTableInheritance())
            {
                statement += ", " + currentClassDef.SuperClassClassDef.TableName;
                foreach (PropDef def in currentClassDef.SuperClassClassDef.PrimaryKeyDef)
                {
					//TODO: Mark - Shouldn't this also have the field Delimiters?
                    where += currentClassDef.SuperClassClassDef.TableName + "." + def.FieldName;
                    where += " = " + currentClassDef.TableName + "." + def.FieldName;
                    where += " AND ";
                }
                currentClassDef = currentClassDef.SuperClassClassDef;
            }
            if (where.Length > 7)
            {
                statement += where.Substring(0, where.Length - 5);
            }

            if (limit > 0)
            {
                statement += " " + _connection.GetLimitClauseForEnd(limit) + " ";
            }
            return statement;
        }

        /// <summary>
        /// Returns the table name
        /// </summary>
        /// <param name="prop">The property</param>
        /// <param name="classDefs">The class definitions</param>
        /// <returns>Returns a string</returns>
        private string GetTableName(BOProp prop, IList classDefs)
        {
            int i = 0;
            bool isSingleTableInheritance = false;
            do
            {
                ClassDef classDef = (ClassDef) classDefs[i];
                if (classDef.IsUsingConcreteTableInheritance())
                {
                    return classDef.TableName;
                }
                else if (classDef.PropDefcol.Contains(prop.PropertyName))
                {
                    if (classDef.SuperClassClassDef == null || classDef.IsUsingClassTableInheritance())
                    {
                        return classDef.TableName;
                    }
                    else if (classDef.IsUsingSingleTableInheritance())
                    {
                        isSingleTableInheritance = true;
                    }
                }
                else if (classDef.IsUsingSingleTableInheritance())
                {
                    isSingleTableInheritance = true;
                }
                else if (isSingleTableInheritance)
                {
                    return classDef.TableName;
                }
                i++;
            } while (i < classDefs.Count);
            return "";
        }

		
    }
}
