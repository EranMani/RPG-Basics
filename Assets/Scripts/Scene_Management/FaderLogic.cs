using System.Collections;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class FaderLogic : MonoBehaviour
    {
        CanvasGroup m_canvasGroup;
        Coroutine currentActiveFade = null;
        
        private void Awake()
        {
            m_canvasGroup = GetComponent<CanvasGroup>();
        }

        public void FadeOutImmediate()
        {
            m_canvasGroup.alpha = 1;
        }
 
        public IEnumerator FadeOutIn()
        {
            yield return FadeOut(3f);
            print("Faded out");
            yield return FadeIn(2f);
            print("Faded in");
        }

        public Coroutine FadeOut(float time)
        {
            return Fade(1, time);
        }

        public Coroutine Fade(float target, float time)
        {
            if (currentActiveFade != null)
            {
                StopCoroutine(currentActiveFade);
            }
            currentActiveFade = StartCoroutine(FadeRoutine(target, time));
            return currentActiveFade;
        }

        public IEnumerator FadeRoutine(float target, float time)
        {
            while (!Mathf.Approximately(m_canvasGroup.alpha, target))
            {
                m_canvasGroup.alpha = Mathf.MoveTowards(m_canvasGroup.alpha, target, Time.deltaTime / time);
                yield return null;
            }
        }

        public Coroutine FadeIn(float time)
        {
            return Fade(0, time);
        }
    }
}


