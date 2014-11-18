using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;

namespace Nodeplay.UI
{

    /// <summary>
    /// behavior to do horrible hack to input field, will force input field
    /// to expand by resizing the background image to the unbounded rect of
    /// it's full text
    /// </summary>
    /// 
    public class ExpandingInputField : UIBehaviour
    {
        public GameObject inputparent;
        InputField inf;

        protected override void Start()
        {

            inf = this.gameObject.GetComponent<InputField>();
            inf.onValueChange.AddListener(new UnityEngine.Events.UnityAction<string>(ResizeInput));
        }

        void ResizeInput(string text)
        {


            Debug.Log("some kind of resizing horror");
            var fullText = inf.text;

            Vector2 extents = inf.textComponent.rectTransform.rect.size;
            var settings = inf.textComponent.GetGenerationSettings(extents);
            settings.generateOutOfBounds = false;
            var prefheight = new TextGenerator().GetPreferredHeight(fullText, settings) + 10;

            if (prefheight > inf.textComponent.rectTransform.rect.height - 10)
            {
                Debug.Log("i grew because the inputfield was only this big" + inf.GetComponent<RectTransform>().rect.height + "and I needed" + prefheight + "space");
                inputparent.GetComponent<LayoutElement>().preferredHeight = prefheight;
            }

        }

    }

}
