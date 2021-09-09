using System.Collections;
using System.Threading.Tasks;

public static class TaskExtensions
{
    public static IEnumerator GetAsIEnumerator(this Task taskToConvert)
    {
        return new TaskEnumerator(taskToConvert);
    }
}

public class TaskEnumerator : IEnumerator
{
    private Task m_task;

    public TaskEnumerator(Task taskToConvert)
    {
        m_task = taskToConvert;
    }

    public bool MoveNext()
    {
        if (!m_task.IsCompleted)
            return true;

        return false;
    }

    public object Current { get; }

    public void Reset()
    {
    }
}
