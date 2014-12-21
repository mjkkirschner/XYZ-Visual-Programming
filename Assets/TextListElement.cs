using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WidgetUI;

public class TextListElement 
	: UIBehaviour
		, IWidget<String>
{
	private Text m_textComponent;
	
	private void Awake()
	{
		m_textComponent = this.GetComponentInChildren<Text>();
	}
	
	public void Enable(string p_text)
	{
		m_textComponent.text = p_text;
	}
	
	public void Disable()
	{
		// ignore
	}
}