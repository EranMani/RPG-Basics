using UnityEngine;
using System;
namespace RPG.Cinematics
{
    public class FakePlayableDirectorLogic : MonoBehaviour
    {
        public event Action<float> onFinish;

        private void Start()
        {
            Invoke("OnFinish", 3f);
        }

        void OnFinish()
        {
            onFinish(3f);
        }
    }
}
