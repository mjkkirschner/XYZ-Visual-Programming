using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using Nodeplay.Interfaces;
using System.ComponentModel;

namespace Nodeplay.Engine
{
    class Graph:MonoBehaviour
    {
        public List<NodeModel> FindNodesWithNoDependencies()
        {
            //list of everything that inherits from nodemodel in the scene
            var allnodes = GameObject.FindObjectsOfType<NodeModel>();
            //list of nodemodels where the input list is empty, so
            // this node has no input ports, or where all inputs are connected...think this does that :P
            var nodeps = allnodes.Where(x => x.Inputs.All(y => y.IsConnected) || x.Inputs.Count == 0).ToList();
            return nodeps;

        }
        /// <summary>
        /// method that triggers evaluation on nodes and validates inputs before triggering 
        /// walks from entry points of computation graph to downstream nodes 
        /// </summary>
        public void EvaluateNodes()
        {

            var entrypoints = FindNodesWithNoDependencies();
            var S = new Stack<NodeModel>(entrypoints);

            while (S.Count > 0)
            {
                var topofstack = S.Peek();
                //TODO this convience method might live on the nodeModel
                if (ReadyForEval(topofstack))
                {
                    Debug.Log(topofstack + "is ready for eval");
                    // then evaluate that sucker
                    //value will be cached on the node for now
                    //TODO what if this node is a data node and requires no evaluation and has no evaluator?
                    topofstack.Evaluate();
                    //pop the evaluated node
                    var popped = S.Pop();
                    //we can now add the nodes that are attached to this nodes outputs
                    var childnodes = popped.Outputs.SelectMany(x => x.connectors.Select(y => y.PEnd.Owner)).ToList();
                    childnodes = childnodes.Except(S).ToList();
                    childnodes.ForEach(x => S.Push(x));
                }
                else
                {
                    //if the node we peeked at was node NOT ready for evaluation then we need to
                    //iterate each of its inputs and evaluate them, this will send a signal
                    //to the node letting us progress to its evaluation next time we look at it in the stack.
                    //We do this by pushing those inputs and possibly their dependencies to the stack here

                    //get all upstream nodes that are not evaluated
                    var parentnodes = topofstack.Inputs.SelectMany(x => x.connectors.Select(y => y.PStart.Owner)).ToList();
                    parentnodes = parentnodes.Where(x => x.StoredValue != null).ToList();
                    parentnodes = parentnodes.Except(S).ToList();
                    //push these parent nodes to the stack
                    // where they will be evaluated
                    parentnodes.ForEach(x => S.Push(x));

                    if (parentnodes.Count != topofstack.Inputs.Count)
                    {
                        //TODO solve this issue, most likely just eval the node and pop it
                        // or return null...?
                        //we have a problem, going to get stuck in infite loop
                        Debug.Break();
                    }
                }


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
                if (inputP.connectors[0].PStart.Owner.StoredValue == null)
                {
                    return false;
                }


            }
            return true;
        }

    }

}

