﻿#region Licensing Header
// ---------------------------------------------------------------------------------
//  Copyright (C) 2007-2011 Chillisoft Solutions
//  
//  This file is part of the Habanero framework.
//  
//      Habanero is a free framework: you can redistribute it and/or modify
//      it under the terms of the GNU Lesser General Public License as published by
//      the Free Software Foundation, either version 3 of the License, or
//      (at your option) any later version.
//  
//      The Habanero framework is distributed in the hope that it will be useful,
//      but WITHOUT ANY WARRANTY; without even the implied warranty of
//      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//      GNU Lesser General Public License for more details.
//  
//      You should have received a copy of the GNU Lesser General Public License
//      along with the Habanero framework.  If not, see <http://www.gnu.org/licenses/>.
// ---------------------------------------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using Habanero.Base;
using Habanero.BO;

namespace Habanero.BO
{
    /// <summary>
    /// A data Accessor used for accessing a remote source.
    /// </summary>
    public class DataAccessorRemote : IDataAccessor
    {
        private readonly IDataAccessor _remoteDataAccessor;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="remoteDataAccessor"></param>
        public DataAccessorRemote(IDataAccessor remoteDataAccessor) {
            _remoteDataAccessor = remoteDataAccessor;
        }

        public ITransactionCommitter CreateTransactionCommitter() {
            return new TransactionCommitterRemote( _remoteDataAccessor.CreateTransactionCommitter());
        }
        public IBusinessObjectLoader BusinessObjectLoader
        {
            get { return _remoteDataAccessor.BusinessObjectLoader; }
        }

    }
   
}
