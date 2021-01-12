using System.Collections.Generic;

namespace Design
{
    public class Menu
    {
        public List<string> Choices { get; } 
        public string Title { get; set; }

        #region Constructors
        public Menu()
        {
            Title = string.Empty;
            Choices = new List<string>();
        }

        public Menu(params string[] choices) : this()
        {
            Create(choices);
        }

        public Menu(string title, params string[] choices) : this(choices)
        {
            Title = title;
        }

        #endregion

        private void Create(params string[] choices)
        {
            // Creating a new menu
            Choices.Clear();
            Add(choices); // calling method AddChoice()
        }

        public void Add(params string[] choices)
        {
            // adding the choices to the list Choices
            foreach (var choice in choices)
                Choices.Add(choice);
        }

        public void AddAtIndex(int index, params string[] choices)
        {
            // adding choice/s at a specific index
            foreach (var choice in choices)
                Choices.Insert(index, choice);
        }

        public IEnumerable<string> Show()
        {
            string sep = (Title.Length % 2 == 0) 
                ? "========================================" : "=======================================";

            int align = ((sep.Length - Title.Length) / 2) + Title.Length;

            if (!string.IsNullOrEmpty(Title))
            {
                yield return string.Format("{0," + align + "}", Title);
                yield return sep;
            }

            foreach (var choice in Choices)
                yield return $"{Choices.IndexOf(choice) + 1}. {choice}";
        }
    }
}