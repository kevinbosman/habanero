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
using Habanero.UI.Base;
using Habanero.UI.VWG;
using NUnit.Framework;

namespace Habanero.Test.UI.Base
{
//    [TestFixture]
    public class TestDataGridViewDateTimePickerColumn //: TestBase
    {
        [SetUp]
        public  void SetupTest()
        {
            //Runs every time that any testmethod is executed
//            base.SetupTest();
        }

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            //Code that is executed before any test is run in this class. If multiple tests
            // are executed then it will still only be called once.
        }

        [TearDown]
        public  void TearDownTest()
        {
            //runs every time any testmethod is complete
//            base.TearDownTest();
        }

        
    }
    public interface IDataGridViewDateTimeColumn : IDataGridViewColumn
    {

        ///// <summary>Gets or sets the underlying value corresponding to a cell value of false, which appears as an unchecked box.</summary>
        ///// <returns>An <see cref="T:System.Object"></see> representing a value that the cells in this column will treat as a false value. The default is null.</returns>
        ///// <exception cref="T:System.InvalidOperationException">The value of the <see cref="IDataGridViewCheckBoxColumn.CellTemplate"></see> property is null. </exception>
        ///// <filterpriority>1</filterpriority>
        //[TypeConverter(typeof(StringConverter)), DefaultValue((string)null)]
        //object FalseValue { get; set; }

        /////// <summary>Gets or sets the flat style appearance of the check box cells.</summary>
        /////// <returns>A <see cref="T:Gizmox.WebGUI.Forms.FlatStyle"></see> value indicating the appearance of cells in the column. The default is <see cref="F:Gizmox.WebGUI.Forms.FlatStyle.Standard"></see>.</returns>
        /////// <exception cref="T:System.InvalidOperationException">The value of the <see cref="IDataGridViewCheckBoxColumn.CellTemplate"></see> property is null. </exception>
        /////// <filterpriority>1</filterpriority>
        ////[DefaultValue(2)]
        ////Gizmox.WebGUI.Forms.FlatStyle FlatStyle { get; set; }

        ///// <summary>Gets or sets the underlying value corresponding to an indeterminate or null cell value, which appears as a disabled checkbox.</summary>
        ///// <returns>An <see cref="T:System.Object"></see> representing a value that the cells in this column will treat as an indeterminate value. The default is null.</returns>
        ///// <exception cref="T:System.InvalidOperationException">The value of the <see cref="IDataGridViewCheckBoxColumn.CellTemplate"></see> property is null. </exception>
        ///// <filterpriority>1</filterpriority>
        //[TypeConverter(typeof(StringConverter)), DefaultValue((string)null)]
        //object IndeterminateValue { get; set; }

        ///// <summary>Gets or sets a value indicating whether the hosted check box cells will allow three check states rather than two.</summary>
        ///// <returns>true if the hosted DataGridViewCheckBoxCell" objects are able to have a third, indeterminate, state; otherwise, false. The default is false.</returns>
        ///// <exception cref="T:System.InvalidOperationException">The value of the DataGridViewCheckBoxColumn.CellTemplate property is null.</exception>
        ///// <filterpriority>1</filterpriority>
        //[DefaultValue(false)]
        //bool ThreeState { get; set; }

        ///// <summary>Gets or sets the underlying value corresponding to a cell value of true, which appears as a checked box.</summary>
        ///// <returns>An <see cref="T:System.Object"></see> representing a value that the cell will treat as a true value. The default is null.</returns>
        ///// <exception cref="T:System.InvalidOperationException">The value of the DataGridViewCheckBoxColumn.CellTemplate property is null.</exception>
        ///// <filterpriority>1</filterpriority>
        //[TypeConverter(typeof(StringConverter)), DefaultValue((string)null)]
        //object TrueValue { get; set; }
    }
    /// <summary>
    /// Represents a column in data grid view
    /// </summary>
    //public class DataGridViewDateTimeColumnVWG :DataGridViewColumnVWG, IDataGridViewDateTimeColumn
    //{
    //    /// <summary>
    //    ///// Constructor to initialise a new column
    //    ///// </summary>
    //    //public DataGridViewDateTimeColumn()
    //    //    : base(new CalendarCell())
    //    //{
    //    //}

    //    ///// <summary>
    //    ///// Gets and sets the cell template
    //    ///// </summary>
    //    //public override IDataGridViewCell CellTemplate
    //    //{
    //    //    get
    //    //    {
    //    //        return base.CellTemplate;
    //    //    }
    //    //    set
    //    //    {
    //    //        // Ensure that the cell used for the template is a CalendarCell.
    //    //        if (value != null &&
    //    //            !value.GetType().IsAssignableFrom(typeof(CalendarCell)))
    //    //        {
    //    //            throw new InvalidCastException("Must be a CalendarCell");
    //    //        }
    //    //        base.CellTemplate = value;
    //    //    }
    //    //}
    //}
}