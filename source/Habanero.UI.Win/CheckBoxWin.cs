using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using Habanero.UI;

namespace Habanero.UI.Win
{
    internal class CheckBoxWin : CheckBox, ICheckBox
    {
        IList IChilliControl.Controls
        {
            get { return this.Controls; }
        }
        //List<IChilliControl> IChilliControl.Controls
        //{
        //    get
        //    {
        //        return new List<IChilliControl>();
        //    }
        //}
    }
}