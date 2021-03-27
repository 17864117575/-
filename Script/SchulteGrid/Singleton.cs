using UnityEngine;

namespace Assets.Script.SchulteGrid
{
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        public static T Instance { get; private set; }

        protected void Awake()
        {
            if(null == Instance)
            {
                Instance = (T)this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
