using Habanero.UI.Forms;
using Habanero.UI.Grid;
using NMock;
using NUnit.Framework;

namespace Habanero.Test.UI.Forms
{
    /// <summary>
    /// Summary description for TestEditableGridButtonControl.
    /// </summary>
    [TestFixture]
    public class TestEditableGridButtonControl
    {
        [Test]
        public void TestControlCreation()
        {
            Mock gridMock = new DynamicMock(typeof (IEditableGrid));
            IEditableGrid grid = (IEditableGrid) gridMock.MockInstance;
            EditableGridButtonControl buttons = new EditableGridButtonControl(grid);
            Assert.AreEqual(2, buttons.Controls.Count);
            Assert.AreEqual("Save", buttons.Controls[1].Name);
            Assert.AreEqual("Cancel", buttons.Controls[0].Name);
        }

        [Test]
        public void TestSaveButtonClick()
        {
            Mock gridMock = new DynamicMock(typeof (IEditableGrid));
            IEditableGrid grid = (IEditableGrid) gridMock.MockInstance;
            gridMock.Expect("AcceptChanges");
            EditableGridButtonControl buttons = new EditableGridButtonControl(grid);
            buttons.ClickButton("Save");
            gridMock.Verify();
        }

        [Test]
        public void TestCancelButtonClick()
        {
            Mock gridMock = new DynamicMock(typeof (IEditableGrid));
            IEditableGrid grid = (IEditableGrid) gridMock.MockInstance;
            gridMock.Expect("RejectChanges");
            EditableGridButtonControl buttons = new EditableGridButtonControl(grid);
            buttons.ClickButton("Cancel");
            gridMock.Verify();
        }
    }
}