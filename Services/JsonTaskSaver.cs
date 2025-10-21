using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Models;
namespace Services
{
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
                List<Models.Task> tasks = new();
                tasks = JsonSerializer.Deserialize<List<Models.Task>>(stream);
                if (tasks == null) { MessageBox.Show("Данных для загрузки нет!", "Ошибка загрузки", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                path.Clear();
                foreach (var task in tasks) path.AddTask(task);
                MessageBox.Show("Успешная загрузка!", "Загрузка задач", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }
    }
}