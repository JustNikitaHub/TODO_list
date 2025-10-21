using Microsoft.VisualBasic;
using System.Reflection;
using Models;
using Services;

namespace TODO_list;


public partial class MainForm : Form
{
    //-----------КОМПОНЕНТЫ
    private Button btnNewTask;
    private Button btnEditTask;
    private Button btnDeleteTask;
    private Button btnSaveJSON;
    private Button btnLoadJSON;
    private ListBox lstTasks;
    private TextBox txtDecriptionTask;
    private CheckBox cbxIsDone;
    private TableLayoutPanel tlpRadioButtons;
    private RadioButton rbnPriorityLow;
    private RadioButton rbnPriorityMedium;
    private RadioButton rbnPriorityHigh;
    private Label lblName;
    private Label lblDescr;
    //-------------Пользовательский классы
    private TaskManager taskManager;
    private JsonTaskSaver saver;
    public MainForm()
    {
        InitializeComponent();
        Size = new Size(800, 600);
        Text = "Планировщик заданий";
        InitializeMyControls();
        btnNewTask.Click += btnNewTask_Click;
        btnDeleteTask.Click += btnDeleteTask_Click;
        btnEditTask.Click += btnEditTask_Click;
        btnSaveJSON.Click += btnSaveJSON_Click;
        btnLoadJSON.Click += btnLoadJSON_Click;
        lstTasks.SelectedIndexChanged += lstTasks_SelectedIndexChanged;
        cbxIsDone.CheckedChanged += cbxIsDone_CheckedChanged;
        rbnPriorityLow.CheckedChanged += rbnPriority0_CheckedChanged;
        rbnPriorityMedium.CheckedChanged += rbnPriority1_CheckedChanged;
        rbnPriorityHigh.CheckedChanged += rbnPriority2_CheckedChanged;
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

    private void rbnPriority2_CheckedChanged(object? sender, EventArgs e)
    {
        taskManager.FindTask(lstTasks.SelectedIndex).ChangePriority((Priority)2);
    }

    private void rbnPriority1_CheckedChanged(object? sender, EventArgs e)
    {
        taskManager.FindTask(lstTasks.SelectedIndex).ChangePriority((Priority)1);
    }

    private void rbnPriority0_CheckedChanged(object? sender, EventArgs e)
    {
        taskManager.FindTask(lstTasks.SelectedIndex).ChangePriority(0);
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
        btnDeleteTask = new()
        {
            Text = "Удалить задачу",
            Location = new Point(50, 505),
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
            Location = new Point(lstTasks.Location.X, lstTasks.Location.Y - 25)
        };
        lblDescr = new()
        {
            Text = "Описание задач",
            AutoSize = true,
            Location = new Point(txtDecriptionTask.Location.X, txtDecriptionTask.Location.Y - 25)
        };
        cbxIsDone = new()
        {
            Text = "Завершена",
            AutoSize = true,
            Location = new Point(txtDecriptionTask.Location.X, txtDecriptionTask.Location.Y + txtDecriptionTask.Height),
            Enabled = false
        };
        tlpRadioButtons = new()
        {
            Location = new Point(cbxIsDone.Location.X, cbxIsDone.Location.Y + cbxIsDone.Height),
            ColumnCount = 1,
            RowCount = 3,
            Enabled = false
        };
        rbnPriorityLow = new()
        {
            AutoSize = true,
            Text = "Низкий приоритет",
            Checked = true
        };
        rbnPriorityMedium = new()
        {
            AutoSize = true,
            Text = "Средний приоритет",
            Checked = false
        };
        rbnPriorityHigh = new()
        {
            AutoSize = true,
            Text = "Высокий приоритет",
            Checked = false
        };
        tlpRadioButtons.Controls.Add(rbnPriorityLow, 0, 0);
        tlpRadioButtons.Controls.Add(rbnPriorityMedium, 0, 1);
        tlpRadioButtons.Controls.Add(rbnPriorityHigh, 0, 2);
        //НЕ ЗАБЫТЬ ДОБАВИТЬ, осел блять!
        Controls.AddRange(
            lstTasks,
            btnNewTask,
            btnEditTask,
            btnDeleteTask,
            btnSaveJSON,
            btnLoadJSON,
            txtDecriptionTask,
            lblName,
            lblDescr,
            cbxIsDone,
            tlpRadioButtons
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
        Models.Task old = taskManager.FindTask(toDel);
        DateTime saveTime = old.CreationTime;
        bool isDone = old.IsCompleted;
        string newName = Interaction.InputBox("Измените имя", "Редактирование", old.Name);
        string newDecsr = Interaction.InputBox("Измените описание", "Редактирование", old.Description);

        //вместо изменения удаляется выбранный Таск и создается копия с новыми данными
        lstTasks.Items.Remove(old.Name);
        taskManager.RemoveTask(old);
        taskManager.CreateTask(newName, newDecsr, saveTime, isDone);
        lstTasks.Items.Add(newName);
    }
    private void btnDeleteTask_Click(object sender, EventArgs e)
    {
        int toDel = lstTasks.SelectedIndex;
        lstTasks.SelectedIndex = -1;
        txtDecriptionTask.Text = "";
        Models.Task old = taskManager.FindTask(toDel);
        lstTasks.Items.Remove(old.Name);
        taskManager.RemoveTask(old);
    }
    private void btnSaveJSON_Click(object sender, EventArgs e)
    {
        saver.Save(taskManager);
    }
    private void btnLoadJSON_Click(object sender, EventArgs e)
    {
        saver.Load(taskManager);
        lstTasks.Items.Clear();
        foreach (var task in taskManager.GetCopy()) lstTasks.Items.Add(task.Name);
    }
    private void lstTasks_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (lstTasks.SelectedIndex == -1)
        {
            btnEditTask.Enabled = false;
            btnDeleteTask.Enabled = false;
            cbxIsDone.Enabled = false;
            tlpRadioButtons.Enabled = false;
            return;
        }
        btnEditTask.Enabled = true;
        btnDeleteTask.Enabled = true;
        cbxIsDone.Enabled = true;
        tlpRadioButtons.Enabled = true;
        var task = taskManager.FindTask(lstTasks.SelectedIndex);
        cbxIsDone.CheckedChanged -= cbxIsDone_CheckedChanged;
        rbnPriorityLow.CheckedChanged -= rbnPriority0_CheckedChanged;
        rbnPriorityMedium.CheckedChanged -= rbnPriority1_CheckedChanged;
        rbnPriorityHigh.CheckedChanged -= rbnPriority2_CheckedChanged;
        (tlpRadioButtons.Controls[(int)task.Priority] as RadioButton).Checked = true;
        rbnPriorityLow.CheckedChanged += rbnPriority0_CheckedChanged;
        rbnPriorityMedium.CheckedChanged += rbnPriority1_CheckedChanged;
        rbnPriorityHigh.CheckedChanged += rbnPriority2_CheckedChanged;
        cbxIsDone.Checked = task.IsCompleted;
        cbxIsDone.CheckedChanged += cbxIsDone_CheckedChanged;
        txtDecriptionTask.Text = task.Description;
    }
    private void cbxIsDone_CheckedChanged(object sender, EventArgs e)
    {
        if (cbxIsDone.Checked) taskManager.FindTask(lstTasks.SelectedIndex).MarkCompleted();
        else taskManager.FindTask(lstTasks.SelectedIndex).MarkUncompleted();
    }




}
