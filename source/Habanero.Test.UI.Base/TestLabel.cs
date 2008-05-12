using System;
using System.Collections.Generic;
using System.Text;
using Habanero.UI;
using Habanero.UI.WebGUI;
using Habanero.UI.Win;
using NUnit.Framework;

namespace Habanero.Test.UI.Base
{

    public abstract class TestLabel
    {
        protected abstract IControlFactory GetControlFactory();

        [TestFixture]
        public class TestLabelWin : TestLabel
        {
            protected override IControlFactory GetControlFactory()
            {
                return new WinControlFactory();
            }
        }

        [TestFixture]
        public class TestLabelGiz : TestLabel
        {
            protected override IControlFactory GetControlFactory()
            {
                return new GizmoxControlFactory();
            }

            [Test]
            public void TestPreferredSize()
            {
                //---------------Set up test pack-------------------
                ILabel myLabel = GetControlFactory().CreateLabel();
                string labelText = "sometext";
                myLabel.Text = labelText;

                //---------------Execute Test ----------------------
                int preferredWidth = myLabel.PreferredWidth;
                //---------------Test Result -----------------------

                Assert.AreEqual(labelText.Length * 8, preferredWidth);
                //---------------Tear Down -------------------------          
            }
        }

        [Test]
        public void TestCreateLabel()
        {
            //---------------Set up test pack-------------------
            //---------------Execute Test ----------------------
            ILabel myLabel = GetControlFactory().CreateLabel();

            //---------------Test Result -----------------------
            Assert.IsNotNull(myLabel);

            //---------------Tear Down -------------------------   
        }

   




    }
}
