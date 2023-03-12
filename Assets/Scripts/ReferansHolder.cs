using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReferansHolder : MonoBehaviour
{
    // This script holds some important script as variable
    public PathManager pathManager;
    public FactoryManager factoryManager;
    public ControllerUI controllerUI;
    public SelectionManager selectionManager;




    // This is the only singleton in this project.
    public static ReferansHolder instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
}
