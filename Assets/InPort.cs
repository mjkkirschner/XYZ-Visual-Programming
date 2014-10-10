using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// object representing an input port.
/// </summary>
public class PortModel : MonoBehaviour
{
	public NodeSimple Owner {get; set;}
	public ConnectorModel Connector {get; set;}
	public int Index {get; set;}

	// Use this for initialization
	void Start ()
	{
		
		
	}
	
	void Update ()
	{
	
	}
}