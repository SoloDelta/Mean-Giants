using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPS
{
    public interface IMoney
    {
        void MoneyPickup(int amount, GameObject obj);
    }
}
