using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;


namespace pract2
{
    public partial class MainWindow : Window
    {
        public class Note
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public DateTime Date { get; set; }

            public Note() { }

            public Note(string title, string description, DateTime date)
            {
                Title = title;
                Description = description;
                Date = date;
            }
        }

        public ObservableCollection<Note> Notes { get; set; } = new ObservableCollection<Note>();
        private Note selectedNote;

        public MainWindow()
        {
            InitializeComponent();
            calendar.SelectedDate = DateTime.Today;
            note.ItemsSource = Notes;
            LoadNotes();
        }

        public static class JsonSerializationDeserialization
        {
            public static void Serialize<T>(T data, string filePath)
            {
                string jsonData = JsonConvert.SerializeObject(data);
                File.WriteAllText(filePath, jsonData);
            }

            public static IEnumerable<T> Deserialize<T>(string filePath)
            {
                if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                {
                    return null;
                }

                try
                {
                    string jsonData = File.ReadAllText(filePath);
                    return JsonConvert.DeserializeObject<IEnumerable<T>>(jsonData);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error while deserializing JSON: " + ex.Message);
                    return null;
                }
            }
        }

        private void SaveNotes()
        {
            JsonSerializationDeserialization.Serialize(Notes, "notes.json");
        }

        private void LoadNotes()
        {
            Notes.Clear();
            IEnumerable<Note> loadedNotes = JsonSerializationDeserialization.Deserialize<Note>("notes.json");
            if (loadedNotes != null)
            {
                foreach (var note in loadedNotes)
                {
                    Notes.Add(note);
                }
                date_SelectedDateChanged(null, null);
            }
        }

        private void date_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime date = calendar.SelectedDate ?? DateTime.Today;
            var filteredNotes = Notes.Where(n => n.Date.Date == date.Date).ToList();
            note.ItemsSource = filteredNotes;
        }

        private void create_note_Click(object sender, RoutedEventArgs e)
        {
            string title = title_note.Text.Trim();
            string description = description_note.Text.Trim();
            DateTime date = calendar.SelectedDate ?? DateTime.Today;

            if (!string.IsNullOrWhiteSpace(title) || !string.IsNullOrWhiteSpace(description))
            {
                Note newNote = new Note(title, description, date);
                Notes.Add(newNote);
                date_SelectedDateChanged(null, null);
                SaveNotes();
            }
        }

        private void delete_note_Click(object sender, RoutedEventArgs e)
        {
            if (selectedNote != null)
            {
                Notes.Remove(selectedNote);
                date_SelectedDateChanged(null, null);
                SaveNotes();
            }
        }

        private void save_note_Click(object sender, RoutedEventArgs e)
        {
            if (selectedNote != null)
            {
                selectedNote.Title = title_note.Text;
                selectedNote.Description = description_note.Text;
                note.Items.Refresh();
                SaveNotes();
            }
        }

        private void note_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedNote = note.SelectedItem as Note;
            if (selectedNote != null)
            {
                title_note.Text = selectedNote.Title;
                description_note.Text = selectedNote.Description;
            }
        }
    }
}