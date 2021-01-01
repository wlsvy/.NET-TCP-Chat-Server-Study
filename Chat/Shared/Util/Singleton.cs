namespace Shared.Util
{
    public class Singleton<T> where T : class, new()
    {
        private static T s_Instance = null;
        public static T I
        {
            get
            {
                if(s_Instance == null)
                {
                    s_Instance = new T();
                }
                return s_Instance;
            }
        }

        public virtual void Destroy()
        {
            s_Instance = null;
        }
    }
}
