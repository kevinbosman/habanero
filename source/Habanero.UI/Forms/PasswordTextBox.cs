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

using System.Windows.Forms;

namespace Habanero.UI.Forms
{
    /// <summary>
    /// Provides a variation of a standard TextBox for passwords, masking
    /// the letters typed with a '*'
    /// </summary>
    public class PasswordTextBox : TextBox
    {
        /// <summary>
        /// Constructor to initialise the TextBox
        /// </summary>
        public PasswordTextBox()
        {
            this.PasswordChar = '*';
        }
    }
}