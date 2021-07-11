/// <summary>
/// Generic implementation of a C# singleton. Used to create a static accessor 
/// for an instance of a class there will only ever be one of.
/// </summary>
/// <typeparam name="T">The class that is being made a singleton</typeparam>
public class Singleton<T> where T : new()
{
    private static T s_instance;

    /// <summary>
    /// Returns an instance of the singleton if it exists already. If not, it creates one via a default constructor
    /// and returns it.
    /// </summary>
    public static T I
    {
        get
        {
            if (s_instance == null)
                s_instance = new T();
            return s_instance;
        }
    }
}
