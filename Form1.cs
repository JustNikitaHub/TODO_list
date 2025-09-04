using System.Text.Unicode;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.VisualBasic;
using System.Reflection;

namespace TODO_list;

public partial class Form1 : Form
{
    //-----------КОМПОНЕНТЫ
    Button btnNewTask;
    Button btnEditTask;
    Button btnSaveJSON;
    Button btnLoadJSON;
    ListBox lstTasks;
    TextBox txtDecriptionTask;
    Label lblName;
    Label lblDescr;
    //-------------
    TaskManager taskManager;
    JsonTaskSaver saver;
    public Form1()
    {
        InitializeComponent();
        Size = new Size(800, 600);
        Text = "Планировщик заданий";
        InitializeMyControls();
        btnNewTask.Click += btnNewTask_Click;
        btnEditTask.Click += btnEditTask_Click;
        btnSaveJSON.Click += btnSaveJSON_Click;
        btnLoadJSON.Click += btnLoadJSON_Click;
        lstTasks.SelectedIndexChanged += lstTasks_SelectedIndexChanged;
        lstTasks.Items.Clear();
        txtDecriptionTask.Text = "";
        saver = new();
        taskManager = new();
        Assembly asm = Assembly.GetExecutingAssembly();
        using (Stream iconStream = asm.GetManifestResourceStream("TODO_list.Resources.icon.ico"))
        {
            if (iconStream != null)
            {
                Icon loadedIcon = new Icon(iconStream);   
                Icon = loadedIcon;
            }
        }
    }
    public void InitializeMyControls()
    {
        btnNewTask = new()
        {
            Text = "Новая задача",
            Location = new Point(50, 470),
            AutoSize = true,
            Anchor = AnchorStyles.Left | AnchorStyles.Bottom
        };
        btnEditTask = new()
        {
            Text = "Изменить задачу",
            Location = new Point(btnNewTask.Location.X + btnNewTask.Size.Width + 80, btnNewTask.Location.Y),
            AutoSize = true,
            Anchor = AnchorStyles.Left | AnchorStyles.Bottom,
            Enabled = false
        };
        btnSaveJSON = new()
        {
            Text = "Сохранить JSON",
            AutoSize = true,
            Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
            Location = new Point(650, 450),
        };
        btnLoadJSON = new()
        {
            Text = "Загрузить JSON",
            AutoSize = true,
            Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
            Location = new Point(650, 500),
        };
        lstTasks = new()
        {
            Location = new Point(50, 50),
            Size = new Size(170, 350)
        };
        txtDecriptionTask = new()
        {
            Multiline = true,
            ReadOnly = true,
            Location = new Point(350, 50),
            Size = new Size(350, 150)
        };
        lblName = new()
        {
            Text = "Список задач",
            AutoSize = true,
            Location = new Point(lstTasks.Location.X,lstTasks.Location.Y-25)
        };
        lblDescr = new()
        {
            Text = "Описание задач",
            AutoSize = true,
            Location = new Point(txtDecriptionTask.Location.X, txtDecriptionTask.Location.Y-25)
        };
        //НЕ ЗАБЫТЬ ДОБАВИТЬ
        Controls.AddRange(
            lstTasks,
            btnNewTask,
            btnEditTask,
            btnSaveJSON,
            btnLoadJSON ,
            txtDecriptionTask,
            lblName,
            lblDescr
            );
    }
    private void btnNewTask_Click(object sender, EventArgs e)
    {
        string nameTask = Interaction.InputBox("Название задачи", "Создание новой задачи", "Пустая задача");
        string descrTask = Interaction.InputBox("Опишите задачу", "Создание новой задачи", "Задача не задана");
        if (taskManager == null)
        {
            string fileName = Interaction.InputBox("Имя папки сохранения", "Создание нового управляющего", "TaskStorage");
            taskManager = new(fileName);
        }
        taskManager.CreateTask(nameTask, descrTask);
        lstTasks.Items.Add(nameTask);
    }
    private void btnEditTask_Click(object sender, EventArgs e)
    {
        int toDel = lstTasks.SelectedIndex;
        lstTasks.SelectedIndex = -1;
        txtDecriptionTask.Text = "";
        Task old = taskManager.FindTask(toDel);
        DateTime saveTime = old.creationTime;
        string newName = Interaction.InputBox("Измените имя", "Редактирование", old.Name);
        string newDecsr = Interaction.InputBox("Измените описание", "Редактирование", old.Description);

        //вместо изменения удаляется выбранный Таск и создается копия с новыми данными
        lstTasks.Items.Remove(old.Name);
        taskManager.RemoveTask(old);
        taskManager.CreateTask(newName, newDecsr, saveTime);
        lstTasks.Items.Add(newName);
    }
    private void btnSaveJSON_Click(object sender, EventArgs e)
    {
        saver.Save(taskManager);
    }
    private void btnLoadJSON_Click(object sender, EventArgs e)
    {
        saver.Load(taskManager);
        foreach (var task in taskManager.GetCopy()) lstTasks.Items.Add(task.Name);
    }
    private void lstTasks_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (lstTasks.SelectedIndex == -1) { btnEditTask.Enabled = false; return; }
        btnEditTask.Enabled = true;
        txtDecriptionTask.Text = taskManager.FindTask(lstTasks.SelectedIndex).Description;
    }
    public class Task
    {
        public int ID { get; init; } = 0;
        public string Name { get; init; } = "Пустая задача";
        public string Description { get; init; } = "Задача не задана";
        private bool IsCompleted { get; set; } = false;
        public DateTime creationTime { get; init; } = DateTime.Now;
        public Task(string name, string description, int id)
        {
            Name = name;
            Description = description;
            ID = id;
            creationTime = DateTime.Now;
        }
        public Task(string name, string description, int id, DateTime date)
        {
            Name = name;
            Description = description;
            ID = id;
            creationTime = date;
        }
        public Task(){}
        public void MarkCompleted()
        {
            IsCompleted = true;
        }
        public bool IsTaskCompleted()
        {
            return IsCompleted;
        }
    }

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
        private void FreeID()
        {
            nextID--;
        }
        public Task FindTask(string name)
        {
            foreach (Task task in tasks)
            {
                if (task.Name == name) return task;
            }
            return null;
        }
        public int FindTask(Task task)
        {
            return tasks.FindIndex(t => t.Equals(task));
        }
        public Task FindTask(int index)
        {
            return tasks[index];
        }
        public void AddTask(Task task) => tasks.Add(task);
        public void CreateTask(string name, string description)
        {
            Task toAdd = new(name, description, nextID);
            AddTask(toAdd);
            PushID();
        }
        public void CreateTask(string name, string description, DateTime date)
        {
            Task toAdd = new(name, description, nextID, date);
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
        public List<Task> GetCopy()
        {
            return tasks;
        }
        public TaskManager(string name)
        {
            FileName = name;
        }
        public TaskManager(){}
    }
    public class JsonTaskSaver
    {
        public void Save(TaskManager path)
        {
            if (path == null) { MessageBox.Show("Данных для записи нет!", "Ошибка записи", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            if (path.CountTasks == 0) { MessageBox.Show("Данных для записи нет!", "Ошибка записи", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            var settings = new JsonSerializerOptions()
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
            };
            using (var stream = new FileStream(path.FileName, FileMode.Create))
            {
                JsonSerializer.Serialize(stream, path.GetCopy(), settings);
            }
        }
        public void Load(TaskManager path)
        {
            using (var stream = new FileStream(path.FileName, FileMode.Open))
            {
                List<Task> tasks = new();
                tasks = JsonSerializer.Deserialize<List<Task>>(stream);
                path.Clear();
                foreach (var task in tasks) path.AddTask(task);
                MessageBox.Show("Успешная загрузка!","Загрузка задач", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            
        }
    }
}
