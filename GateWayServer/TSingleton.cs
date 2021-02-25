namespace GateWayServer
{
    public class TSingleton<T> where T : class, new()
    {
        public static T Instance
        {
            get;
            private set;
        }

        static TSingleton()
        {
            if (Instance == null)
            {
                Instance = new T();
            }
        }

        public virtual void Clear()
        {
            Instance = null;
            Instance = new T();
        }

        public static T CreateInstance()
        {
            return Instance;
        }
    }
}
