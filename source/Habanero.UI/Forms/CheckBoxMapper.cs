//---------------------------------------------------------------------------------
// Copyright (C) 2007 Chillisoft Solutions
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
using System.Windows.Forms;
using Habanero.UI.Base;

namespace Habanero.UI.Forms
{
    /// <summary>
    /// This mapper represents a CheckBox object in a user interface
    /// </summary>
    public class CheckBoxMapper : ControlMapper
    {
        private CheckBox _checkBox;

        /// <summary>
        /// Constructor to create a new CheckBox mapper object
        /// </summary>
        /// <param name="cb">The CheckBox object to be mapped</param>
        /// <param name="propName">A name for the property</param>
        /// <param name="isReadOnly">Whether this control is read only</param>
        public CheckBoxMapper(CheckBox cb, string propName, bool isReadOnly) 
			: base(cb, propName, isReadOnly)
        {
            _checkBox = cb;
            _checkBox.CheckedChanged += new EventHandler(ValueChangedHandler);
        }

        /// <summary>
        /// The event handler that is called when the user has checked or
        /// unchecked the CheckBox.
        /// </summary>
        /// <param name="sender">The object that sent the message about a
        /// change in value</param>
        /// <param name="e">Attached arguments regarding the event</param>
        private void ValueChangedHandler(object sender, EventArgs e)
        {
			if (!_isEditable) return;

            bool newValue = _checkBox.Checked;
            bool valueChanged = false;
            if (GetPropertyValue() == null)
            {
                valueChanged = true;
            }
            else
            {
                bool oldValue = Convert.ToBoolean(GetPropertyValue());
                if (newValue != oldValue)
                {
                    valueChanged = true;
                }
            }
            if (valueChanged)
            {
                //log.Debug("setting property value to " + newValue + " of type bool");
                SetPropertyValue(newValue);
                //_businessObject.SetPropertyValue(_propertyName, newValue);
            }
        }

        /// <summary>
        /// Updates the appearance of the object when the value of the
        /// property has changed internally
        /// </summary>
        protected override void ValueUpdated()
        {
            object propValue = GetPropertyValue();
            bool newValue;
            if (propValue != null)
            {
                newValue = Convert.ToBoolean(propValue);
            }
            else
            {
                newValue = false;
            }
            if (newValue != _checkBox.Checked)
            {
                _checkBox.Checked = newValue;
            }
        }
    }
}