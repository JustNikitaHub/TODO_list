namespace Models
{
    public class Task
    {
        public int ID { get; init; } = 0;
        public string Name { get; set; } = "Пустая задача";
        public string Description { get; set; } = "Задача не задана";
        public bool IsCompleted { get; set; } = false;
        public DateTime CreationTime { get; init; } = DateTime.Now;
        public Priority Priority { get; set; } = Priority.Low;
        public Task() : this("Пустая задача", "Задача не задана", 0) { }
        public Task(string name, string description, int id) : this(name, description, id, DateTime.Now) { }
        public Task
        (
            string name,
            string description,
            int id,
            DateTime date,
            Priority priority = Priority.Low,
            bool isCompleted = false
        )
        {
            Name = name;
            Description = description;
            ID = id;
            CreationTime = date;
            Priority = priority;
            IsCompleted = isCompleted;
        }
        public void MarkCompleted() => IsCompleted = true;
        public bool GetTaskCompleteStatus() => IsCompleted;
        public void MarkUncompleted() => IsCompleted = false;
        public void ChangePriority(Priority priority) => Priority = priority;
        public override string ToString()
        {
            return $"ID: {ID}, Name: {Name}, Description: {Description}, Priority: {Priority}, Completed: {IsCompleted}";
        }
    }

    public enum Priority
    {
        Low,
        Medium,
        High
    }
}