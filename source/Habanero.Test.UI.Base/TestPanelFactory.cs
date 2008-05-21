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

using Habanero.BO.ClassDefinition;
using Habanero.UI.Base;
using NUnit.Framework;

namespace Habanero.Test.UI.Base
{
    /// <summary>
    /// Summary description for TestPanelFactory.
    /// </summary>
    public abstract class TestPanelFactory
    {
        protected abstract IControlFactory GetControlFactory();

        protected abstract void ApplyChangesToBusinessObject(IPanelFactoryInfo info);

        //[TestFixture]
        //public class TestPanelFactoryWin : TestPanelFactory
        //{
        //    protected override IControlFactory GetControlFactory()
        //    {
        //        return new Habanero.UI.Win.ControlFactoryWin();
        //    }

        //    protected override void ApplyChangesToBusinessObject(IPanelFactoryInfo info)
        //    {
        //        // do nothing - on windows the changes should be applied automatically when a value in a control changes
        //    }
        //}

        [TestFixture]
        public class TestPanelFactoryGiz : TestPanelFactory
        {
            protected override IControlFactory GetControlFactory()
            {
                return new Habanero.UI.WebGUI.ControlFactoryGizmox();
            }

            [SetUp]
            public void SetupTest()
            {
                ClassDef.ClassDefs.Clear();
                Sample.CreateClassDefGiz();
            }

            protected override void ApplyChangesToBusinessObject(IPanelFactoryInfo info)
            {
           
            info.ControlMappers.ApplyChangesToBusinessObject();
       
            }
        }

        [Test]
        public void TestOnePropertyForm()
        {   
            Sample s = new Sample();
            IPanelFactory factory = new PanelFactory(s, GetControlFactory());
            IPanel pnl = factory.CreatePanel().Panel;
            Assert.AreEqual(2, pnl.Controls.Count, "The panel should have 2 controls - one label and one text box.");
            Assert.IsTrue(pnl.Controls[0] is ILabel);
            Assert.AreEqual("Text:", pnl.Controls[0].Text);
            Assert.IsTrue(pnl.Controls[1] is ITextBox);
        }

        [Test]
        public void TestMapperIsConnected()
        {
            Sample s = new Sample();
            IPanelFactory factory = new PanelFactory(s, Sample.SampleUserInterfaceMapperGiz.SampleUserInterfaceMapper3Props(), GetControlFactory());
            IPanelFactoryInfo info = factory.CreatePanel();
            IPanel pnl = info.Panel;
            ITextBox tb = (ITextBox)info.ControlMappers["SampleText"].Control;
            tb.Text = "Test";
            ApplyChangesToBusinessObject(info);
            
            Assert.AreEqual("Test", s.SampleText);
        }


        [Test]
        public void TestAlternateConstructor()
        {
            Sample s = new Sample();
            IPanelFactory factory = new PanelFactory(s, new Sample.SampleUserInterfaceMapperGiz().GetUIFormProperties(), GetControlFactory());
            IPanel pnl = factory.CreatePanel().Panel;
            Assert.AreEqual(2, pnl.Controls.Count, "The panel should have 2 controls - one label and one text box.");
            Assert.IsTrue(pnl.Controls[0] is ILabel);
            Assert.IsTrue(pnl.Controls[1] is ITextBox);
        }

        [Test]
        public void TestWithMoreThanOneProperty()
        {
            Sample s = new Sample();
            IPanelFactory factory = new PanelFactory(s, Sample.SampleUserInterfaceMapperGiz.SampleUserInterfaceMapper3Props(), GetControlFactory());
            IPanelFactoryInfo pnlInfo = factory.CreatePanel();
            IPanel pnl = pnlInfo.Panel;
            Assert.AreEqual(6, pnl.Controls.Count, "The panel should have 6 controls.");
            Assert.AreEqual(3, pnlInfo.ControlMappers.Count, "The PanelInfo should have 3 mappers");
            Assert.IsTrue(pnl.Controls[0] is ILabel);
            int labelWidth = pnl.Controls[0].Width;
            Assert.IsTrue(pnl.Controls[1] is ITextBox);
            Assert.IsTrue(pnl.Controls[2] is ILabel);
            Assert.AreEqual(labelWidth, pnl.Controls[2].Width);
            Assert.IsTrue(pnl.Controls[3] is ITextBox);
            Assert.IsTrue(pnl.Controls[4] is ILabel);
            Assert.AreEqual(labelWidth, pnl.Controls[4].Width);
            Assert.IsTrue(pnl.Controls[5] is ITextBox);
        }

        [Test]
        public void TestWithOnePrivateProperty()
        {
            Sample s = new Sample();
            IPanelFactory factory = new PanelFactory(s, Sample.SampleUserInterfaceMapperGiz.SampleUserInterfaceMapperPrivatePropOnly(), GetControlFactory());
            IPanelFactoryInfo pnlInfo = factory.CreatePanel();
            IPanel pnl = pnlInfo.Panel;
            Assert.AreEqual(2, pnl.Controls.Count, "The panel should have 2 controls.");
            Assert.AreEqual(1, pnlInfo.ControlMappers.Count, "The PanelInfo should have 1 mappers");
            Assert.IsTrue(pnl.Controls[0] is ILabel);
            Assert.IsTrue(pnl.Controls[1] is ITextBox);

            Assert.AreEqual('*', ((ITextBox)pnl.Controls[1]).PasswordChar);
        }

        [Test]
        public void TestToolTipWithOneDescribedProperty()
        {
            Sample s = new Sample();
            IPanelFactory factory = new PanelFactory(s, Sample.SampleUserInterfaceMapperGiz.SampleUserInterfaceMapperDescribedPropOnly(null), GetControlFactory());
            IPanelFactoryInfo pnlInfo = factory.CreatePanel();
            IPanel pnl = pnlInfo.Panel;
            Assert.AreEqual(2, pnl.Controls.Count, "The panel should have 2 controls.");
            Assert.AreEqual(1, pnlInfo.ControlMappers.Count, "The PanelInfo should have 1 mappers");
            Assert.IsTrue(pnl.Controls[0] is ILabel);
            Assert.IsTrue(pnl.Controls[1] is ITextBox);
            IToolTip toolTip = pnlInfo.ToolTip;
            string toolTipText;
            //The label should have the description of the property as it's tooltip.
            toolTipText = toolTip.GetToolTip((IControlChilli)pnl.Controls[0]);
            Assert.AreSame("This is a sample text property that has a description.", toolTipText);
            //The textbox should also have the description of the property as it's tooltip.
            toolTipText = toolTip.GetToolTip((IControlChilli)pnl.Controls[1]);
            Assert.AreSame("This is a sample text property that has a description.", toolTipText);
        }

        [Test]
        public void TestToolTipWithOneDescribedPropertyWithSpecifiedToolTip()
        {
            Sample s = new Sample();
            string controlToolTipText = "This is my control with a tool tip.";
            IPanelFactory factory = new PanelFactory(s, Sample.SampleUserInterfaceMapperGiz.SampleUserInterfaceMapperDescribedPropOnly(controlToolTipText), GetControlFactory());
            IPanelFactoryInfo pnlInfo = factory.CreatePanel();
            IPanel pnl = pnlInfo.Panel;
            Assert.AreEqual(2, pnl.Controls.Count, "The panel should have 2 controls.");
            Assert.AreEqual(1, pnlInfo.ControlMappers.Count, "The PanelInfo should have 1 mappers");
            Assert.IsTrue(pnl.Controls[0] is ILabel);
            Assert.IsTrue(pnl.Controls[1] is ITextBox);
            IToolTip toolTip = pnlInfo.ToolTip;
            string toolTipText;
            //The label should have the description of the property as it's tooltip.
            toolTipText = toolTip.GetToolTip(pnl.Controls[0]);
            Assert.AreSame(controlToolTipText, toolTipText);
            //The textbox should also have the description of the property as it's tooltip.
            toolTipText = toolTip.GetToolTip(pnl.Controls[1]);
            Assert.AreSame(controlToolTipText, toolTipText);
        }

        [Test]
        public void TestWithMoreThanOneColumn()
        {
            Sample s = new Sample();
            IPanelFactory factory = new PanelFactory(s, Sample.SampleUserInterfaceMapperGiz.SampleUserInterfaceMapper2Cols(), GetControlFactory());
            IPanel pnl = factory.CreatePanel().Panel;
            Assert.AreEqual(8, pnl.Controls.Count, "The panel should have 8 controls.");
            int labelWidth = pnl.Controls[0].Width;
            int leftPos = pnl.Controls[0].Left;
            Assert.AreEqual(labelWidth, pnl.Controls[4].Width, "All labels in the column should be the same width"); // control 4 is first one on second row
            Assert.AreEqual(leftPos, pnl.Controls[4].Left, "All labels in the column should be positioned at the same x position");
            Assert.IsTrue(pnl.Controls[2].Left > leftPos + labelWidth, "New column should be started here");
            int column1width = 100;
            int bordersize = 5;
            int gapSize = 2;
            int column2width = 150;
            int column2left = column1width + bordersize + (gapSize*2);
            Assert.AreEqual(column2left, pnl.Controls[2].Left);
            Assert.IsTrue(pnl.Controls[3] is ITextBox);
            Assert.AreEqual(column2left + column2width + 2, pnl.Controls[3].Left + pnl.Controls[3].Width); //  column 2 width is 150
        }

        [Test]
        public void TestWithMoreThanOneTab()
        {
            Sample s = new Sample();
            IPanelFactory factory = new PanelFactory(s, Sample.SampleUserInterfaceMapperGiz.SampleUserInterfaceMapper2Tabs(), GetControlFactory());
            IPanelFactoryInfo factoryInfo = factory.CreatePanel();
            IPanel pnl = factoryInfo.Panel;
            Assert.AreEqual(1, pnl.Controls.Count, "The panel should have 1 control.");
            Assert.AreEqual(3, factoryInfo.ControlMappers.Count);
            Assert.IsTrue(pnl.Controls[0] is ITabControl, "The control should be a tabcontrol");
            ITabControl tabControl = (ITabControl) pnl.Controls[0];
            Assert.AreEqual(2, tabControl.TabPages.Count, "There should be 2 tabs");
            Assert.AreEqual("mytab1", tabControl.TabPages[0].Text);
            Assert.AreEqual("mytab2", tabControl.TabPages[1].Text);
        }

        [Test]
        public void TestPanelInfoAndPanelSizes()
        {
            Sample s = new Sample();
            IPanelFactory factory = new PanelFactory(s, Sample.SampleUserInterfaceMapperGiz.SampleUserInterfaceMapper3Props(), GetControlFactory());
            IPanelFactoryInfo pnlInfo = factory.CreatePanel();
            Assert.AreEqual(300, pnlInfo.PreferredHeight);
            Assert.AreEqual(350, pnlInfo.PreferredWidth);
            Assert.IsTrue(pnlInfo.Panel.Height < 300);
        }

        [Test]
        public void TestReadOnlyFields()
        {
            Sample s = new Sample();
            IPanelFactory factory = new PanelFactory(s, GetControlFactory());
            IPanel pnl = factory.CreatePanel().Panel;
            Assert.AreEqual(2, pnl.Controls.Count, "The panel should have 2 controls - one label and one text box.");
            Assert.IsTrue(pnl.Controls[1] is ITextBox);
            ITextBox tb = (ITextBox) pnl.Controls[1];
            Assert.IsFalse(tb.Enabled);
        }

        [Test]
        public void TestMultiLineTextBox()
        {
            Sample s = new Sample();
            IPanelFactory factory = new PanelFactory(s, GetControlFactory());
            IPanel pnl = factory.CreatePanel().Panel;
            Assert.IsTrue(pnl.Controls[1] is ITextBox);
            ITextBox tb = (ITextBox) pnl.Controls[1];
            Assert.IsTrue(tb.Multiline, "Textbox should be multiline if NumLines > 1");
            ITextBox myTb =  GetControlFactory().CreateTextBox();
            Assert.AreEqual(myTb.Height*3, tb.Height);
        }

        [Test, Ignore("Row spanning seems a little dodge")]
        public void TestColumnSpanningTextBox()
        {
            Sample s = new Sample();
            IPanelFactory factory = new PanelFactory(s, Sample.SampleUserInterfaceMapperGiz.SampleUserInterfaceMapperColSpanning(), GetControlFactory());
            IPanel pnl = factory.CreatePanel().Panel;
            ILabel lbl = (ILabel) pnl.Controls[0];
            ITextBox tb = (ITextBox)pnl.Controls[1];
            ITextBox tbInSecondColumn = (ITextBox)pnl.Controls[3];
            Assert.AreEqual(tbInSecondColumn.Left + tbInSecondColumn.Width, tb.Left + tb.Width);
        }

        [Test]
        public void TestRowSpanningTextBox()
        {
            Sample s = new Sample();
            IPanelFactory factory = new PanelFactory(s, Sample.SampleUserInterfaceMapperGiz.SampleUserInterfaceMapperRowSpanning(), GetControlFactory());
            IPanel pnl = factory.CreatePanel().Panel;
            ITextBox tb = (ITextBox) pnl.Controls[1];
            Assert.IsTrue(tb.Multiline, "Textbox should be multiline if NumLines > 1");

            ITextBox tb2 = (ITextBox) pnl.Controls[7];
            int textboxHeight = 20;
            int numLines = 3;
            int borderSize = 5;
            int gapSize = 2;
            Assert.AreEqual(numLines * textboxHeight + borderSize + gapSize, tb2.Top);
        }
    }
}