//---------------------------------------------------------------------------------
// Copyright (C) 2008 Chillisoft Solutions
// 
// This file is part of the Habanero framework.
// 
//     Habanero is a free framework: you can redistribute it and/or modify
//     it under the terms of the GNU Lesser General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     The Habanero framework is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU Lesser General Public License for more details.
// 
//     You should have received a copy of the GNU Lesser General Public License
//     along with the Habanero framework.  If not, see <http://www.gnu.org/licenses/>.
//---------------------------------------------------------------------------------

using System;
using Habanero.Base;

namespace Habanero.BO.ClassDefinition
{
    /// <summary>
    /// Used to implement custom type conversions specifically for DateTime objects.
    /// </summary>
    public class BOPropDateTimeDataMapper : BOPropDataMapper
    {
        public override string ConvertValueToString(object value)
        {
            if (value == null) return "";
            object parsedPropValue;
            TryParsePropValue(value, out parsedPropValue);
            if (parsedPropValue is DateTime) return ((DateTime) parsedPropValue).ToString(_standardDateTimeFormat);

            return parsedPropValue == null ? "" : parsedPropValue.ToString();
        }

        public override bool TryParsePropValue(object valueToParse, out object returnValue)
        {
            if (base.TryParsePropValue(valueToParse, out returnValue)) return true;

            if (!(valueToParse is DateTime))
            {
                if (valueToParse is DateTimeToday || valueToParse is DateTimeNow)
                {
                    returnValue = valueToParse;
                    return true;
                }
                if (valueToParse is String)
                {
                    string stringValueToConvert = (string)valueToParse;
                    if (stringValueToConvert.ToUpper() == "TODAY")
                    {
                        returnValue = new DateTimeToday();
                        return true;
                    }
                    if (stringValueToConvert.ToUpper() == "NOW")
                    {
                        returnValue = new DateTimeNow();
                        return true;
                    }
                }
                DateTime dateTimeOut;
                if (DateTime.TryParse(valueToParse.ToString(), out dateTimeOut))
                {
                    returnValue = dateTimeOut;
                    return true;
                }
                returnValue = null;
                return false;
            }
            returnValue = valueToParse;
            return true;
        }
    }
}