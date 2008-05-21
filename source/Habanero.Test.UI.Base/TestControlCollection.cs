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

using Habanero.UI.Base;
using Habanero.UI.WebGUI;
using Habanero.UI.Win;
using NUnit.Framework;

namespace Habanero.Test.UI.Base
{
    public abstract class TestControlCollection
    {
        protected abstract IControlFactory GetControlFactory();

        //[TestFixture]
        //public class TestControlCollectionWin : TestControlCollection
        //{
        //    protected override IControlFactory GetControlFactory()
        //    {
        //        return new ControlFactoryWin();
        //    }
        //}

        [TestFixture]
        public class TestControlCollectionGiz : TestControlCollection
        {
            protected override IControlFactory GetControlFactory()
            {
                return new ControlFactoryGizmox();
            }
        }
        [Test]
        public void TestAddControl()
        {
            TextBoxGiz tb = (TextBoxGiz) GetControlFactory().CreateTextBox();
            IControlCollection col = new ControlCollectionGiz(new Gizmox.WebGUI.Forms.Control.ControlCollection(tb));
            IControlChilli ctl = GetControlFactory().CreateControl();
            col.Add(ctl);
            Assert.AreSame(ctl, col[0], "Control added should be the same object.");
        }

        //[Test]
        //public void TestAddNull()
        //{
        //    IControlCollection col = new ControlCollectionChilli();
        //    col.Add(null);
        //}
    }
}