using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Nodeplay.Interfaces;

namespace Nodeplay.Nodes
{
		public class TestNodeType : NodeModel
		{
	
				
				protected override void Start ()
				{
						base.Start ();
						AddOutPutPort ();
						AddInputPort ();
				
				}
	
				void Update ()
				{
		
				}
	
	
	
		}
}
