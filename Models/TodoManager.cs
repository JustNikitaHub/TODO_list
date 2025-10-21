namespace Models
{
    public class TaskManager
    {
        private List<Task> tasks = new();
        private int nextID = 0;
        public int CountTasks => tasks.Count;
        public string FileName = "TaskStorage";
        private void PushID()
        {
            if (nextID == tasks[tasks.Count - 1].ID) nextID++;
        }
        private void FreeID() => nextID--;
        public Task FindTask(string name)
        {
            return tasks.FirstOrDefault(t => t.Name == name);
        }
        public int FindTask(Task task)
        {
            return tasks.FindIndex(t => t.Equals(task));
        }
        public Task FindTask(int index) => tasks[index];
        public void AddTask(Task task) => tasks.Add(task);
        public void CreateTask(string name, string description)
        {
            Task toAdd = new(name, description, nextID);
            AddTask(toAdd);
            PushID();
        }
        public void CreateTask(string name, string description, DateTime date, bool isDone = false)
        {
            Task toAdd = new(name, description, nextID, date);
            if (isDone) toAdd.MarkCompleted();
            AddTask(toAdd);
            PushID();
        }
        public void RemoveTask(Task task)
        {
            tasks.Remove(task);
            FreeID();
        }
        public void Clear()
        {
            tasks.Clear();
            nextID = 0;
        }
        public List<Task> GetCopy() => tasks;
        public TaskManager(string name) => FileName = name;
        public TaskManager() { }
    }
}