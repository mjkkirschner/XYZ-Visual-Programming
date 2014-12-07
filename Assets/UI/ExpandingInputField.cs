using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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
		private Regex colorTags = new Regex("<[^>]*>");
		private Regex keyWordsBlue = new Regex("if |then |else |fi |true |while |do |done |set |export |bool |break |case |class |const |for |foreach |goto |in |void |if\n|then\n|else\n|fi\n|true\n|while\n|do\n|done\n|set\n|export\n|bool\n|break\n|case\n|class\n|const\n|for\n|foreach\n|goto\n|in\n|void\n");
		

        protected override void Start()
        {

            inf = this.gameObject.GetComponent<InputField>();
            inf.onValueChange.AddListener(new UnityEngine.Events.UnityAction<string>(ResizeInput));
			inf.onEndEdit.AddListener(new UnityEngine.Events.UnityAction<string>(highlight));
			
        }


		public void highlight(string text){
			
				inf.text = colorTags.Replace(inf.text, @"");
				inf.text = keyWordsBlue.Replace(inf.text, @"<color=blue>$&</color>");
				inf.MoveTextEnd(false);
			
			
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
