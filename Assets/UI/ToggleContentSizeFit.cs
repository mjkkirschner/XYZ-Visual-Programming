using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Nodeplay.Interfaces;
using System.ComponentModel;
using UnityEngine.UI;

namespace Nodeplay.UI.Utils
{
    class ToggleContentSizeFit : MonoBehaviour
    {
        public void onValueChanged()
        {   //switch the current state of the contentfitter based on previous state
            if (this.GetComponent<ContentSizeFitter>().verticalFit
                == ContentSizeFitter.FitMode.PreferredSize)
            {
                this.GetComponent<ContentSizeFitter>().verticalFit =
                    ContentSizeFitter.FitMode.MinSize;

            }
            else
            {
                this.GetComponent<ContentSizeFitter>().verticalFit =
               ContentSizeFitter.FitMode.PreferredSize;
            }

        }
    }
}
