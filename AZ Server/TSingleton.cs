using System;

namespace AZ_Server
{
    public class TSingleton<T> where T : class, new()
    {
        public static T Instance { get; private set; }

        static TSingleton()
        {
            if (Instance == null)
            {
                Instance = Activator.CreateInstance<T>();
            }
        }

        public virtual void Clear()
        {
            Instance = default;
            Instance = Activator.CreateInstance<T>();
        }

        public static T CreateInstance()
        {
            return Instance;
        }
    }
}
