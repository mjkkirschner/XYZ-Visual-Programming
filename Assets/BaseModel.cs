using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Nodeplay.Interfaces;
using System.ComponentModel;
using System;

public class BaseModel : MonoBehaviour, INotifyPropertyChanged
{
    //todo probably will need to readd location properties if I want to support the non-graph based workflows...$$$

  

    public event PropertyChangedEventHandler PropertyChanged;
    private void NotifyPropertyChanged(String info)
    {
        Debug.Log("sending some property change notification");
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
    }

    protected virtual void Start()
    {

       

    }
    /// <summary>
    /// this code will either be a method or expression for generating UI elements
    /// it may also point to UI prefab data to load
    /// the viewmodel will execute this code, it is stored here to avoid needing custom views
    /// for each element type with similar interaction logic
    /// </summary>
    protected virtual void BuildSceneElements()
    {



    }

    
    

 

  




}
