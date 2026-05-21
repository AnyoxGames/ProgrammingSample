using System.Collections.Generic;
using AnyoxGames.Service;
using AnyoxGames.Util;
using UnityEngine;

namespace AnyoxGames.UI
{
    public class HUD : MonoBehaviourService
    {
        //Use of the Odin Inspector package, commented out as it's not included with this project
        [ /*ReadOnly, */SerializeField] private List<AHUDBehaviour> AllBehaviours;

        public bool HasHUDBehaviour<T>() where T : AHUDBehaviour
        {
            foreach (var behaviour in AllBehaviours)
                if (behaviour is T)
                {
                    return true;
                }

            return false;
        }

        public bool TryGetHUDBehaviour<T>(out T behaviour) where T : AHUDBehaviour
        {
            behaviour = null;

            foreach (var otherBehaviour in AllBehaviours)
            {
                if (otherBehaviour is not T characterBehaviour)
                    continue;

                behaviour = characterBehaviour;
                break;
            }

            return behaviour;
        }

        //Use of the Odin Inspector package, commented out as it's not included with this project
        /*[Button("Revalidate Behaviours")]
        public void RevalidateBehaviours()
        {
            AllBehaviours = GetComponentsInChildren<AHUDBehaviour>().ToList();
            foreach (var hudBehaviour in AllBehaviours)
            {
                hudBehaviour.HUD = this;
            }
        }*/
    }
}