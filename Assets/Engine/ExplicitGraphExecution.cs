using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using Nodeplay.Interfaces;
using System.ComponentModel;
using Nodeplay.UI.Utils;
using Nodeplay.Nodes;
using System;

namespace Nodeplay.Engine
{
	public class ExplicitGraphExecution:MonoBehaviour
	{
		public List<Task> TaskSchedule;
		public Task CurrentTask;
		public float ExecutionsPerFrame {get;set;}
		public Boolean  ButtonPressed {get;set;}
		public UnityEngine.UI.Toggle DebugModeToggle;
		private List<GameObject> SceneState;
		private bool running = false;
		public event Action Evaluating; 
		public List<ControlFlowDelegateNodeModel> updaters = new List<ControlFlowDelegateNodeModel>();
		private bool updateloop = false;

		protected void OnEvaluating()
		{
			if (Evaluating != null){

			Evaluating();
			}
		} 

		public event Action GraphEvaluationStarted;
		protected void OnGraphEvaluatonStart()
		{
			if (GraphEvaluationStarted != null){
				
				GraphEvaluationStarted();
			}
		} 
		protected virtual void Update()
		{
			if (running){

				updaters.ForEach(x=> TaskSchedule.Add( new Task(null,x,0, new Action(()=>{x.Evaluate();}), new WaitForEndOfFrame())));
				                 }
		}

		protected virtual void Start()
		{
			ExecutionsPerFrame =1;
			DebugModeToggle = GameObject.Find("DebugToggle").GetComponent<UnityEngine.UI.Toggle>();
		}

		public List<NodeModel> FindNodesWithNoDependencies()
		{
			//list of everything that inherits from nodemodel in the scene
			var allnodes = GameObject.FindObjectsOfType<NodeModel>();
			//list of nodemodels where the input list is empty, so
			// this node has no input ports, or where all inputs are connected...think this does that :P
			var nodeps = allnodes.Where(x => x.ExecutionInputs.Count == 0).ToList();
			nodeps.ForEach(x=>Debug.Log(x.name));
			return nodeps;
			
		}
		
		public List<NodeModel> FindEntryPoints()
		{	//TODO anything where we using find objects of type on nodemodels or graph elements is going to break with custom nodes....
			//need to only search in specific graph model... or use a flag...
			//list of everything that inherits from nodemodel in the scene
			var allnodes = GameObject.FindObjectsOfType<NodeModel>();
			//list of nodemodels where the input list is empty, so
			// this node has no input ports, or where all inputs are connected...think this does that :P
			var nodeps = allnodes.Where(x => x.ExecutionInputs.Count == 0 && !(x is CreateVariable ||x is InputExecutionNode || x is Nodes.OnUpdate) || x is StartExecution).ToList();
			//nodeps.ForEach(x=>Debug.Log(x.name));
			return nodeps;
			
		}
		

		/// <summary>
		/// method that triggers evaluation on nodes and validates inputs before triggering 
		/// walks from entry points of computation graph to downstream nodes 
		/// </summary>
		public void EvaluateNodes(bool state)
		{
			running = state;
			if (state){
			StartCoroutine("ObservableEval");
			}
		}
		/// <summary>
		/// method that checks if a node is ready for eval
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		private bool ReadyForEval(NodeModel node)
		{
			foreach (var inputP in node.Inputs)
			{   //TODO add null check for connector
				Debug.Log("checking "+ inputP.NickName + "on" + node);
				Debug.Log(inputP.connectors.Count);
				if (inputP.connectors[0].PStart.Owner.StoredValueDict == null)
				{
					return false;
				}
				
				
			}
			return true;
		}
		
		IEnumerator WaitForButtonPress(UnityEngine.UI.Button button)
		{
			ButtonPressed = false;
			Debug.Log("setting button pressed to false");
			while(ButtonPressed != true)
				{
				//don't do anything...
				yield return null;
				}
			}
		private List<GameObject> FindRootGameobjects(){
		
		List<GameObject> rootObjects = new List<GameObject>();
			foreach (Transform xform in UnityEngine.Object.FindObjectsOfType<Transform>())
			{
				if (xform.parent == null)
				{
					rootObjects.Add(xform.gameObject);
				}
			}
			return rootObjects;
		}

		/// <summary>
		/// generator that evals a new node each frame
		/// issue here is that we are actually slowing evaluation 
		/// instead of just slowing the updates to the nodes based on their position
		/// in the eval graph
		/// </summary>
		/// <returns></returns>
		IEnumerator ObservableEval()
		{
			OnGraphEvaluatonStart();
			if (SceneState != null)
			{
			//delete everything from the last run
				var CreatedGameObjects =  FindRootGameobjects().Except(FindRootGameobjects().Where(x=>x.GetComponentInChildren<BaseModel>() != null));
			CreatedGameObjects = CreatedGameObjects.Except(SceneState);
			CreatedGameObjects.ToList().ForEach(x=>Destroy(x));
			}
			//on run get the current state of the scene
			SceneState = FindRootGameobjects();

			var entrypoints = FindEntryPoints();
			List<Task> actions = entrypoints.Select(x=> new Task(null,x,0,new System.Action (() => x.Evaluate()), new WaitForSeconds(1))).ToList();
			TaskSchedule = new List<Task>(actions);
	
			Task headOfQueue = null;
			while (running)
			{
				//do not gointo infinte loop...
				if (!(TaskSchedule.Count > 0))
				{
					yield return new WaitForEndOfFrame();
				}

				foreach (var i in Enumerable.Range(0,Math.Min((int)ExecutionsPerFrame,TaskSchedule.Count)).ToList())
				{
					headOfQueue = TaskSchedule.First();

					//pop the node we are about to evaluate, otherwise we'll never be able to 
					//IDEA, if this tasks.... => headOfQueue.NodeRunningOn.ExecutionRoot().GetType() != typeof(OnUpdate)
					//is true then go through a forloop until thats not true, this way we'll execute all the nodes, but then
					//we'll yield?... thats the behavior we need, not sure this will get it..


					OnEvaluating();
					CurrentTask = headOfQueue;
					headOfQueue.MethodCall.Invoke();
					TaskSchedule.RemoveAt(0);

					//if we're the current task is running on a node that was called by an updater
					if (headOfQueue.NodeRunningOn.ExecutionRoot().GetType() == typeof(OnUpdate))
					{
						//just keep looping, do not yield
						updateloop = true;

					}
					else{
						updateloop = false;
					}
			

					//now check state of debug mode, if it's enabled move the camera to the location of the executing node
					//and simply poll the state of the continue button, do not continue until button pressed.
					if (DebugModeToggle.isOn)
					{
						var nodepos = headOfQueue.NodeRunningOn.transform.position;
						var offsettpos = nodepos + (headOfQueue.NodeRunningOn.transform.right * 20f);

						//now calculate where the camera is currently looking
						var cameraviewpoint = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 30f));

						StartCoroutine(slowmove(Camera.main.transform.position,offsettpos,cameraviewpoint,nodepos,1f,Camera.main.gameObject));
						//will need to subclass button so that it holds state or use a toggle...
						//for now just hold state on this execution class, //TODO fix this
						yield return StartCoroutine(WaitForButtonPress(null));
					}


				}

				if (updateloop && ! (CurrentTask.NodeRunningOn.GetType() == typeof(OnUpdate))){
					continue;
				}

				if (headOfQueue.Yieldbehavior != null ){
					yield return headOfQueue.Yieldbehavior;
				}
			}
		}
		//TODO this does not belong here
		private IEnumerator slowmove(Vector3 frompos,Vector3 topos,Vector3 lookFrom,Vector3 lookat, float duration,GameObject goToMove)
		{
			Debug.Log("moving camera");

			for (float f = 0; f <= duration; f = f + Time.deltaTime)
			{
				//move the camera towards the new node
				goToMove.transform.position = Vector3.Lerp(frompos, topos, f);

				goToMove.transform.LookAt(Vector3.Lerp(lookFrom,lookat,f));

				yield return null;
				
			}
			//goToMove.transform.position = topos;
		}

		
	}
	
	
}





