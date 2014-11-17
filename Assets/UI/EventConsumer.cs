using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;

namespace Nodeplay .UI
{   ///<summary>
    ///this behavior consumes specific events that are bubbling up heirarchy
    ///TODO add a UI for this behavior so user can set which events to consume at this point
    /// </summary>
    class EventConsumer:EventTrigger
    {
        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            eventData.Use();
        }

    }
}
