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
    class Graph
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
                    // then evaluate that sucker
                    //value will be cached on the node for now
                    topofstack.Evaluate();
                    //pop the evaluated node
                    var popped = S.Pop();
                    //we can now add the nodes that are attached to this nodes outputs
                    var childnodes = popped.Outputs.SelectMany(x => x.connectors.Select(y => y.PEnd.Owner)).ToList();
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
                    //push these parent nodes to the stack
                    // where they will be evaluated
                    parentnodes.ForEach(x => S.Push(x));
                }


            }

        }
        /// <summary>
        /// method that checks if 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private bool ReadyForEval(NodeModel node)
        {
            foreach (var inputP in node.Inputs)
            {
                if (inputP.Owner.StoredValue == null)
                {
                    return false;
                }


            }
            return true;
        }

    }

}

