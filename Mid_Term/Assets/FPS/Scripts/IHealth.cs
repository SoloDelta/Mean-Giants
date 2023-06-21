using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FPS
{
    public interface IHealth
    {
        void healthPickup(int amount, GameObject obj);
    }

}
