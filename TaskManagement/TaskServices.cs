namespace TaskManagement
{
    public class TaskServices
    {
        public bool IsTaskCompleted(int taskId)
        {
            return taskId % 2 == 0;
        }
    }
}
