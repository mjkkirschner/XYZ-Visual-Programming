using UnityEngine;

public class ListInit : MonoBehaviour
{
	public MyList m_list;
	
	void Start()
	{
		// fill the list
		for(int i=0; i<40; ++i)
		{
			m_list.Add("Element " + i);
		}
		
		// register a callback
		m_list.OnSelectItem.AddListener(this.OnSelectItem);
	}
	
	void OnSelectItem(string p_item)
	{
		Debug.Log("Selected item: " + p_item);
	}
}