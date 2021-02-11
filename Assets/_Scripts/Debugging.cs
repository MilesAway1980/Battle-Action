using UnityEngine;
using System.Collections;

public class Debugging : MonoBehaviour
{
    public bool debug = false;
    static Debugging self;

    void Start()
    {
        if (self == null)
        {
            self = this;
        }
    }

    public static bool DebugMode()
    {
        return self.debug;
    }
}
