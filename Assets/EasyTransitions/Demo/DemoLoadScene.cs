using UnityEngine;

namespace EasyTransition
{

    public class DemoLoadScene : MonoBehaviour
    {
        public TransitionSettings transition;
        public float startDelay;
        public GameObject player;
        
        public void LoadScene(string _sceneName)
        {
            TransitionManager.Instance.Transition(_sceneName, transition, startDelay);
        }

        public void LoadPosition()
        {
            TransitionManager.Instance.Transitions(transition, startDelay, player, new Vector2(5f, 2f));
        }

    }

}


