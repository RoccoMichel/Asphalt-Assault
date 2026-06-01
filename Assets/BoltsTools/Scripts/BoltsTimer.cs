using System;
using System.Collections;
using UnityEngine;

namespace BoltsTools
{
    public class BoltsTimer
    {
        static bool isFrozen;
        static float timeScale;
        
        /// <summary>
        /// Freezes The Frame
        /// </summary>
        /// <param name="sec">The Number Of Seconds Frozen</param>
        public static IEnumerator FreezeFrame(float sec)
        {
            float oldTimeScale = Time.timeScale;
            Time.timeScale = 0;
            yield return new WaitForSecondsRealtime(sec);
            Time.timeScale = oldTimeScale;
        }

        /// <summary>
        /// Waits Until Condition Is True
        /// </summary>
        /// <param name="T">The Condition(s)</param>
        /// <param name="F">The Action To Run</param>
        /// <returns></returns>
        public static IEnumerator WaitFor(Func<bool> T, Action F)
        {
            yield return new WaitUntil(T);
            F?.Invoke();
        }

        /// <summary>
        /// Waits To Do An Action
        /// </summary>
        /// <param name="sec">The Number Of Seconds To Wait</param>
        /// <param name="F">The Action To Run</param>
        /// <returns></returns>
        public static IEnumerator WaitForSeconds(float sec, Action F)
        {
            yield return new WaitForSeconds(sec);
            F?.Invoke();
        }

        /// <summary>
        /// Waits For Animation To Finish
        /// </summary>
        /// <param name="animator">The Animator That Is Playing The Animation</param>
        /// <param name="clipName">The Animation Clip Name</param>
        /// <param name="F">The Action To Run</param>
        /// <returns></returns>
        public static IEnumerator WaitForAnimation(Animator animator, string clipName, Action F)
        {
            while (!animator.GetCurrentAnimatorStateInfo(0).IsName(clipName))
                yield return null;

            while (animator.GetCurrentAnimatorStateInfo(0).IsName(clipName) &&
                   animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
                yield return null;
            
            F?.Invoke();
        }

        
        /// <summary>
        /// Freezes/Unfreezes The Game For Debugging
        /// </summary>
        public static void FreezeGame()
        {
            isFrozen = !isFrozen;
            if (!isFrozen)
                timeScale = Time.timeScale;
            Time.timeScale = isFrozen ? 0 : timeScale;
        }

        /// <summary>
        /// Frame Steps When You Called The FreezeGame Void
        /// </summary>
        /// <param name="frames">Numbers Of Frames To Step</param>
        /// <returns></returns>
        public static IEnumerator FrameStep(int frames)
        {
            if (isFrozen)
            {
                int stepped = 0;
                Time.timeScale = 1;

                while (stepped < frames)
                {
                    yield return null;
                    stepped++;
                }

                Time.timeScale = 0;
            }
            else
            {
                Debug.LogError("Game Was Not Frozen! Call The FreezeGame Void!");
            }
        }

        /// <summary>
        /// Waits X Frames And Checks If A Condition Is True And Then Runs An Action
        /// </summary>
        /// <param name="condition">Condition To Run The Action</param>
        /// <param name="frames">Max Frames To Wait Before Stoping</param>
        /// <param name="toDo">Action To Do</param>
        /// <returns></returns>
        public static IEnumerator HoldInput(Func<bool> condition, int frames, Action toDo)
        {
            int framesWaited = 0;
            while (framesWaited <= frames)
            {
                framesWaited++;
                
                Debug.Log(framesWaited);

                yield return new WaitForEndOfFrame();
                
                if(condition())
                {
                    toDo?.Invoke();
                    break;
                }
            }
        }
    }
}
