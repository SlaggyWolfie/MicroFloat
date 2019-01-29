using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slavi
{
    public class CoroutineHelpers
    {
        public static IEnumerator WaitOneFrame(Action action)
        {
            yield return null;
            action();
        }

        public static IEnumerator WaitXFrames(int frames, Action action)
        {
            for (int i = frames; i > 0; i--) yield return null;
            action();
        }

        public static IEnumerator WaitForEndOfFrame(Action action)
        {
            yield return new WaitForEndOfFrame();
            action();
        }

        public static IEnumerator Wait(float seconds = 1)
        {
            yield return new WaitForSeconds(seconds);
        }

        public static IEnumerator Wait(float seconds, Action action)
        {
            yield return new WaitForSeconds(seconds);
            action.Invoke();
        }

        public static IEnumerator Wait<T>(float seconds, Action<T> action, T parameter)
            where T : class
        {
            yield return new WaitForSeconds(seconds);
            action.Invoke(parameter);
        }

        public static IEnumerator DoWhile(Func<bool> pred, Action @while, Action after = null, bool updateEveryFrame = true)
        {
            while (pred != null && pred())
            {
                if (updateEveryFrame) yield return null;
                if (@while != null) @while.Invoke();
            }

            if (after != null) after.Invoke();
        }

        public static IEnumerator DoUntil(Func<bool> pred, Action until, Action after = null, bool updateEveryFrame = true)
        {
            while (pred != null && !pred())
            {
                if (updateEveryFrame) yield return null;
                if (until != null) until.Invoke();
            }

            if (after != null) after.Invoke();
        }

        public static IEnumerator WaitUntil(Func<bool> predicate, Action action)
        {
            yield return new WaitUntil(predicate);
            action();
        }

        public static IEnumerator WaitWhile(Func<bool> predicate, Action action)
        {
            yield return new WaitWhile(predicate);
            action();
        }
    }

    public sealed class ComponentStatus
    {
        private ComponentStatus() { }
        public static void SetComponentEnabled(Component component, bool value = true)
        {
            if (component == null) return;
            if (component is Renderer) ((Renderer)component).enabled = value;
            else if (component is Collider) ((Collider)component).enabled = value;
            else if (component is Behaviour) ((Behaviour)component).enabled = value;
            else Debug.Log("Don't know how to enable " + component.GetType().Name);
        }

        public static bool IsEnabled(Component component)
        {
            if (component == null)
            {
                Debug.LogError("Component is null.");
                return false;
            }

            bool value = false;
            if (component is Renderer) { value = ((Renderer)component).enabled; }
            else if (component is Collider) { value = ((Collider)component).enabled; }
            else if (component is Behaviour) { value = ((Behaviour)component).enabled; }
            else { Debug.LogWarning("Don't know if was " + component.GetType().Name + " enabled."); }

            return value;
        }
    }
}