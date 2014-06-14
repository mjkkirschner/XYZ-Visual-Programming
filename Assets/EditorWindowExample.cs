//using UnityEngine;
//using UnityEditor;
//using System.Collections;
//using System.Collections.Generic;
//
//
//public class EditorWindowExample : EditorWindow
//{
//	List<Node> nodes = new List<Node> ();
//
//
//	[MenuItem ("Window/EditorWindow example")]
//	static void Launch ()
//	{
//		GetWindow<EditorWindowExample> ().title = "Example";
//	}
//
//
//	void OnGUI ()
//	{
//		GUILayout.Label ("This is an editor window - the base of any completely custom GUI work.", EditorStyles.wordWrappedMiniLabel);
//
//		// Render all connections first //
//
//		if (Event.current.type == EventType.repaint)
//		{
//			foreach (Node node in nodes)
//			{
//				foreach (Node target in node.Targets)
//				{
//					Node.DrawConnection (node.Position, target.Position);
//				}
//			}
//		}
//
//		GUI.changed = false;
//
//		foreach (Node node in nodes)
//		// Handle all nodes
//		{
//			node.OnGUI ();
//		}
//
//		wantsMouseMove = Node.Selection != null;
//			// If we have a selection, we're doing an operation which requires an update each mouse move
//
//		switch (Event.current.type)
//		{
//			case EventType.mouseUp:
//			// If we had a mouse up event which was not handled by the nodes, clear our selection
//				Node.Selection = null;
//				Event.current.Use ();
//			break;
//			case EventType.mouseDown:
//				if (Event.current.clickCount == 2)
//				// If we double-click and no node handles the event, create a new node there
//				{
//					Node.Selection = new Node ("Node " + nodes.Count, Event.current.mousePosition);
//					nodes.Add (Node.Selection);
//					Event.current.Use ();
//				}
//			break;
//		}
//
//		if (GUI.changed)
//		// Repaint if we changed anything
//		{
//			Repaint ();
//		}
//	}
//}
