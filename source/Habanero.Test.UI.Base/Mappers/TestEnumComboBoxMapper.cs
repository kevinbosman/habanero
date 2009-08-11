﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.UI.Base;
using Habanero.UI.Win;
using NUnit.Framework;

namespace Habanero.Test.UI.Base.Mappers
{
    [TestFixture]
    public class TestEnumComboBoxMapper
    {
        private const string ENUM_PROP_NAME = "EnumProp";
        private const string ENUM_PROP_NAME_EMPTY = "EnumPropEmpty";
        private const string ENUM_PKPROP_NAME = "EnumPropPK";

        [SetUp]
        public void Setup()
        {
            ClassDef.ClassDefs.Clear();
            ClassDef.ClassDefs.Add(GetClassDef());
        }

        [Test]
        public void Test_SetupComboBoxItems_PopulatesComboBoxWithEnum()
        {
            //---------------Set up test pack-------------------
            EnumComboBoxMapper enumComboBoxMapper= CreateComboBox(ENUM_PROP_NAME);
            ComboBoxWin comboBox = (ComboBoxWin)enumComboBoxMapper.Control;
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            enumComboBoxMapper.SetupComboBoxItems();
            //---------------Test Result -----------------------
            Assert.AreEqual(4, comboBox.Items.Count);
            Assert.AreEqual("", comboBox.Items[0].ToString());
            Assert.AreEqual(TestEnum.Option1.ToString(), comboBox.Items[1].ToString());
            Assert.AreEqual(TestEnum.Option2.ToString(), comboBox.Items[2].ToString());
            Assert.AreEqual(TestEnum.Option3.ToString(), comboBox.Items[3].ToString());
        }

        [Test]
        public void Test_SetupComboBoxItems_PopulatesComboBoxWithEmptyEnum_StillWorksFine()
        {
            //---------------Set up test pack-------------------
            EnumComboBoxMapper enumComboBoxMapper = CreateComboBox(ENUM_PROP_NAME_EMPTY);
            ComboBoxWin comboBox = (ComboBoxWin)enumComboBoxMapper.Control;
            //---------------Assert Precondition----------------
            Assert.AreEqual(0, Enum.GetNames(typeof (TestEnumEmpty)).Length);
            //---------------Execute Test ----------------------
            enumComboBoxMapper.SetupComboBoxItems();
            //---------------Test Result -----------------------
            Assert.AreEqual(1, comboBox.Items.Count);
            Assert.AreEqual("", comboBox.Items[0].ToString());
        }

        [Test]
        public void Test_SetupComboBoxItems_ExceptionThrownIfPropertyTypeNotEnum()
        {
            //---------------Set up test pack-------------------
            EnumComboBoxMapper enumComboBoxMapper = CreateComboBox(ENUM_PKPROP_NAME);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            try
            {
                enumComboBoxMapper.SetupComboBoxItems();
                //---------------Test Result -----------------------
                Assert.Fail("Expected to throw an InvalidPropertyException");
            } 
            catch (InvalidPropertyException ex)
            {
                StringAssert.Contains("EnumComboBoxMapper can only be used for an enum property type", ex.Message);
            }
        }

        [Test]
        public void Test_InternalUpdateControlValueFromBo_IfNull_SelectsBlank()
        {
            //---------------Set up test pack-------------------
            EnumComboBoxMapper enumComboBoxMapper = CreateComboBox(ENUM_PROP_NAME);
            ComboBoxWin comboBox = (ComboBoxWin)enumComboBoxMapper.Control;
            EnumBO enumBO = (EnumBO) enumComboBoxMapper.BusinessObject;
            enumComboBoxMapper.SetupComboBoxItems();
            //---------------Assert Precondition----------------
            Assert.AreEqual(-1, comboBox.SelectedIndex);
            //---------------Execute Test ----------------------
            enumBO.EnumProp = null;
            //---------------Test Result -----------------------
            Assert.AreEqual(0, comboBox.SelectedIndex);
        }

        [Test]
        public void Test_InternalUpdateControlValueFromBo_IfNotNull_SelectsNonBlank()
        {
            //---------------Set up test pack-------------------
            EnumComboBoxMapper enumComboBoxMapper = CreateComboBox(ENUM_PROP_NAME);
            ComboBoxWin comboBox = (ComboBoxWin)enumComboBoxMapper.Control;
            EnumBO enumBO = (EnumBO)enumComboBoxMapper.BusinessObject;
            enumComboBoxMapper.SetupComboBoxItems();
            //---------------Assert Precondition----------------
            Assert.AreEqual(-1, comboBox.SelectedIndex);
            //---------------Execute Test ----------------------
            enumBO.EnumProp = TestEnum.Option3;
            //---------------Test Result -----------------------
            Assert.AreEqual(3, comboBox.SelectedIndex);
        }

        [Test]
        public void Test_ApplyChangesToBusinessObject_SelectBlank_SetsPropertyToNull()
        {
            //---------------Set up test pack-------------------
            EnumComboBoxMapper enumComboBoxMapper = CreateComboBox(ENUM_PROP_NAME);
            ComboBoxWin comboBox = (ComboBoxWin)enumComboBoxMapper.Control;
            EnumBO enumBO = (EnumBO)enumComboBoxMapper.BusinessObject;
            enumComboBoxMapper.SetupComboBoxItems();
            enumBO.EnumProp = TestEnum.Option3;
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, comboBox.SelectedIndex);
            Assert.AreEqual(TestEnum.Option3, enumBO.EnumProp.Value);
            //---------------Execute Test ----------------------
            comboBox.SelectedIndex = 0;
            //---------------Test Result -----------------------
            Assert.IsFalse(enumBO.EnumProp.HasValue);
        }

        [Test]
        public void Test_ApplyChangesToBusinessObject_SelectMinusOne_SetsPropertyToNull()
        {
            //---------------Set up test pack-------------------
            EnumComboBoxMapper enumComboBoxMapper = CreateComboBox(ENUM_PROP_NAME);
            ComboBoxWin comboBox = (ComboBoxWin)enumComboBoxMapper.Control;
            EnumBO enumBO = (EnumBO)enumComboBoxMapper.BusinessObject;
            enumComboBoxMapper.SetupComboBoxItems();
            enumBO.EnumProp = TestEnum.Option3;
            //---------------Assert Precondition----------------
            Assert.AreEqual(3, comboBox.SelectedIndex);
            Assert.AreEqual(TestEnum.Option3, enumBO.EnumProp.Value);
            //---------------Execute Test ----------------------
            comboBox.SelectedIndex = -1;
            //---------------Test Result -----------------------
            Assert.IsFalse(enumBO.EnumProp.HasValue);
        }

        [Test]
        public void Test_ApplyChangesToBusinessObject_SelectNonBlank_SetsPropertyToNonNull()
        {
            //---------------Set up test pack-------------------
            EnumComboBoxMapper enumComboBoxMapper = CreateComboBox(ENUM_PROP_NAME);
            ComboBoxWin comboBox = (ComboBoxWin)enumComboBoxMapper.Control;
            EnumBO enumBO = (EnumBO)enumComboBoxMapper.BusinessObject;
            enumComboBoxMapper.SetupComboBoxItems();
            enumBO.EnumProp = null;
            //---------------Assert Precondition----------------
            Assert.AreEqual(0, comboBox.SelectedIndex);
            Assert.IsFalse(enumBO.EnumProp.HasValue);
            //---------------Execute Test ----------------------
            comboBox.SelectedIndex = 3;
            //---------------Test Result -----------------------
            Assert.IsTrue(enumBO.EnumProp.HasValue);
            Assert.AreEqual(TestEnum.Option3, enumBO.EnumProp.Value);
        }

        private static EnumComboBoxMapper CreateComboBox(string propertyName)
        {
            EnumBO bo = new EnumBO();
            ComboBoxWin comboBox = new ComboBoxWin();
            IControlFactory controlFactory = new ControlFactoryWin();
            EnumComboBoxMapper enumComboBoxMapper = new EnumComboBoxMapper(comboBox, propertyName, false, controlFactory);
            enumComboBoxMapper.BusinessObject = bo;
            return enumComboBoxMapper;
        }

        private static ClassDef GetClassDef()
        {
            PropDef propDefPK = new PropDef(ENUM_PKPROP_NAME, typeof(Guid), PropReadWriteRule.WriteNew, null);
            PropDef propDef = new PropDef(ENUM_PROP_NAME, typeof(TestEnum), PropReadWriteRule.ReadWrite, TestEnum.Option1);
            PropDef propDef2 = new PropDef(ENUM_PROP_NAME_EMPTY, typeof(TestEnumEmpty), PropReadWriteRule.ReadWrite, null);
            PrimaryKeyDef primaryKeyDef = new PrimaryKeyDef { propDefPK };
            PropDefCol propDefCol = new PropDefCol { propDefPK, propDef, propDef2 };

            UIFormField uiFormField = new UIFormField(TestUtil.GetRandomString(), propDef.PropertyName,
                typeof(ComboBox), "EnumComboBoxMapper", "Habanero.UI.Base", true, null, null, LayoutStyle.Label);
            UIFormColumn uiFormColumn = new UIFormColumn { uiFormField };
            UIFormTab uiFormTab = new UIFormTab { uiFormColumn };
            UIForm uiForm = new UIForm { uiFormTab };
            UIDef uiDef = new UIDef("default", uiForm, null);
            UIDefCol uiDefCol = new UIDefCol { uiDef };

            ClassDef classDef = new ClassDef(typeof(EnumBO), primaryKeyDef, propDefCol, new KeyDefCol(), null, uiDefCol);
            return classDef;
        }

        public enum TestEnum
        {
            Option1,
            Option2,
            Option3
        }

        public enum TestEnumEmpty
        {
            
        }

        public class EnumBO : BusinessObject
        {
            public virtual TestEnum? EnumProp
            {
                get
                {
                    return ((TestEnum?)(base.GetPropertyValue(ENUM_PROP_NAME)));
                }
                set
                {
                    base.SetPropertyValue(ENUM_PROP_NAME, value);
                }
            }
        }
    }
}
