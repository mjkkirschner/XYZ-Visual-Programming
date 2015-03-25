using UnityEngine;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using UnityEngine.UI;
using Nodeplay.Engine;
using Nodeplay.Nodes;
using System.Linq;
using System.Text.RegularExpressions;

public class Search : MonoBehaviour
{

	public InputField input;
	public Library library;
	public List<GameObject> searchResults = new List<GameObject>();

	public void Start()
	{
		input = GetComponentInChildren<InputField>();
		library = transform.root.GetComponentInChildren<Library>();

		input.onValueChange.AddListener(new UnityEngine.Events.UnityAction<string>(UpdateSearchResults));
	}

	
	void UpdateSearchResults (string searchText)
	{
		searchResults = new List<GameObject>();
		Debug.Log("updating search result");
		Debug.Log(searchText);

		if (String.IsNullOrEmpty(searchText))
		{
			foreach(var button in library.buttons)
			{
				button.SetActive(true);
			}

			return;
		}

		//turn all buttons off
		foreach(var button in library.buttons)
		{
			button.SetActive(false);
		}

		foreach(GameObject button in library.buttons)
		{

			if (button.GetComponent<LibraryButton>().NameLabel.text.ToLower().StartsWith(searchText) ||
			    button.GetComponent<LibraryButton>().NameLabel.text.ToLower().Contains(searchText) || 
			    Regex.Matches(button.GetComponent<LibraryButton>().NameLabel.text.ToLower(), searchText).Count > 0)
			{
				searchResults.Add(button);

			}
		}
		//for all buttons found, turn them on
		foreach(GameObject button in searchResults)
		{
			button.SetActive(true);
		}
	}
}