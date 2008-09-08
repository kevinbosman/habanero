using System;
using System.Reflection;
using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.UI.Base;
using log4net;

namespace Habanero.UI.Win
{
    /// <summary>
    /// Provides a form used to edit business objects.  This form will usually
    /// be constructed using a UI Form definition provided in the class definitions.
    /// The appropriate UI definition is typically set in the constructor.
    /// </summary>
    public class DefaultBOEditorFormWin : FormWin, IDefaultBOEditorForm
    {
        private static readonly ILog log = LogManager.GetLogger("Habanero.UI.Win.DefaultBOEditorFormWin");
        private BusinessObject _bo;
        private IControlFactory _controlFactory;
        private string _uiDefName;
        private IPanelFactoryInfo _panelFactoryInfo;
        private IPanel _boPanel;
        private IButtonGroupControl _buttons;
        private PostObjectPersistingDelegate _action;


        public DefaultBOEditorFormWin(BusinessObject bo, string name, IControlFactory controlFactory, PostObjectPersistingDelegate action)
            : this(bo, name, controlFactory)
        {
            _action = action;
            
        }

        public DefaultBOEditorFormWin(BusinessObject bo, string uiDefName, IControlFactory controlFactory)
        {
            _bo = bo;
            _controlFactory = controlFactory;
            _uiDefName = uiDefName;

            BOMapper mapper = new BOMapper(bo);

            UIForm def;
            if (_uiDefName.Length > 0)
            {
                UIDef uiMapper = mapper.GetUIDef(_uiDefName);
                if (uiMapper == null)
                {
                    throw new NullReferenceException("An error occurred while " +
                                                     "attempting to load an object editing form.  A possible " +
                                                     "cause is that the class definitions do not have a " +
                                                     "'form' section for the class, under the 'ui' " +
                                                     "with the name '" + _uiDefName + "'.");
                }
                def = uiMapper.GetUIFormProperties();
            }
            else
            {
                UIDef uiMapper = mapper.GetUIDef();
                if (uiMapper == null)
                {
                    throw new NullReferenceException("An error occurred while " +
                                                     "attempting to load an object editing form.  A possible " +
                                                     "cause is that the class definitions do not have a " +
                                                     "'form' section for the class.");
                }
                def = uiMapper.GetUIFormProperties();
            }
            if (def == null)
            {
                throw new NullReferenceException("An error occurred while " +
                                                 "attempting to load an object editing form.  A possible " +
                                                 "cause is that the class definitions do not have a " +
                                                 "'form' section for the class.");
            }

            IPanelFactory factory = new PanelFactory(_bo, def, _controlFactory);
            _panelFactoryInfo = factory.CreatePanel();
            _boPanel = _panelFactoryInfo.Panel;
            _buttons = _controlFactory.CreateButtonGroupControl();
            _buttons.AddButton("Cancel", CancelButtonHandler);
            IButton okbutton = _buttons.AddButton("OK", OKButtonHandler);
            okbutton.NotifyDefault(true);
            this.AcceptButton = (ButtonWin)okbutton;
            this.Load += delegate { FocusOnFirstControl(); };

            Text = def.Title;
            SetupFormSize(def);
            MinimizeBox = false;
            MaximizeBox = false;
            //this.ControlBox = false;

            CreateLayout();
            OnResize(new EventArgs());
        }

        protected IPanel BoPanel
        {
            get { return _boPanel; }
        }

        private void CreateLayout()
        {
            BorderLayoutManager borderLayoutManager = new BorderLayoutManagerWin(this, _controlFactory);
            borderLayoutManager.AddControl(BoPanel, BorderLayoutManager.Position.Centre);
            borderLayoutManager.AddControl(Buttons, BorderLayoutManager.Position.South);
        }

        private void SetupFormSize(UIForm def)
        {
            int width = def.Width;
            int minWidth = _boPanel.Width +
                           Margin.Left + Margin.Right;
            if (width < minWidth)
            {
                width = minWidth;
            }
            int height = def.Height;
            int minHeight = _boPanel.Height + _buttons.Height +
                            Margin.Top + Margin.Bottom;
            if (height < minHeight)
            {
                height = minHeight;
            }
            Height = height;
            Width = width;
        }

        private void FocusOnFirstControl()
        {
            IControlChilli controlToFocus = _panelFactoryInfo.FirstControlToFocus;
            MethodInfo focusMethod = controlToFocus.GetType().
                GetMethod("Focus", BindingFlags.Instance | BindingFlags.Public);
            if (focusMethod != null)
            {
                focusMethod.Invoke(controlToFocus, new object[] { });
            }
        }

        private void OKButtonHandler(object sender, EventArgs e)
        {
            try
            {
                _panelFactoryInfo.ControlMappers.ApplyChangesToBusinessObject();
                TransactionCommitter committer = CreateSaveTransaction();
                committer.CommitTransaction();
                DialogResult = Base.DialogResult.OK;
                Close();
                if (_action != null)
                {
                    _action(this._bo);
                }
                _panelFactoryInfo.ControlMappers.BusinessObject = null;
            }
            catch (Exception ex)
            {
                log.Error(ExceptionUtilities.GetExceptionString(ex, 0, true));
                GlobalRegistry.UIExceptionNotifier.Notify(ex, "There was a problem saving for the following reason(s):",
                    "Saving Problem");
            }
        }

        protected virtual TransactionCommitter CreateSaveTransaction()
        {
            TransactionCommitterDB committer = new TransactionCommitterDB();
            committer.AddBusinessObject(_bo);
            return committer;
        }

        private void CancelButtonHandler(object sender, EventArgs e)
        {
            _panelFactoryInfo.ControlMappers.BusinessObject = null;
            _bo.Restore();
            DialogResult = Base.DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// Gets the button control for the buttons in the form
        /// </summary>
        public IButtonGroupControl Buttons
        {
            get { return _buttons; }
        }

        /// <summary>
        /// Gets or sets the dialog result that indicates what action was
        /// taken to close the form
        /// </summary>
        public Base.DialogResult DialogResult
        {
            get { return (Base.DialogResult) base.DialogResult; }
            set { base.DialogResult = (System.Windows.Forms.DialogResult)value; }
        }

        /// <summary>
        /// Gets the object containing all information related to the form, including
        /// its controls, mappers and business object
        /// </summary>
        public IPanelFactoryInfo PanelFactoryInfo
        {
            get { return _panelFactoryInfo; }
        }

        /// <summary>
        /// Pops the form up in a modal dialog.  If the BO is successfully edited and saved, returns true,
        /// else returns false.
        /// </summary>
        /// <returns>True if the edit was a success, false if not</returns>
        bool IDefaultBOEditorForm.ShowDialog()
        {
            {
                if (this.ShowDialog() == (System.Windows.Forms.DialogResult)Base.DialogResult.OK)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}