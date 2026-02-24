using System;
using System.Windows;
using WpfApp1.Data;
using WpfApp1.Models;

namespace WpfApp1;

public partial class AuthorWindow : Window
{
    public Author Author { get; set; }

    // Конструктор теперь принимает контекст, как и BookWindow
    public AuthorWindow(Author? a, LibraryContext context)
    {
        InitializeComponent();
        
        if (a != null)
        {
            Author = a;
            TxtFirstName.Text = a.FirstName;
            TxtLastName.Text = a.LastName;
            TxtCountry.Text = a.Country;
            DpBirth.SelectedDate = a.BirthDate;
        }
        else
        {
            // По умолчанию ставим дату 30 лет назад
            Author = new Author { BirthDate = DateTime.Now.AddYears(-30) };
            DpBirth.SelectedDate = Author.BirthDate;
        }
    }

    private void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        // 1. Валидация текстовых полей
        if (string.IsNullOrWhiteSpace(TxtFirstName.Text) || string.IsNullOrWhiteSpace(TxtLastName.Text))
        {
            MessageBox.Show("Имя и Фамилия обязательны для заполнения!");
            return;
        }

        // 2. Валидация даты рождения
        if (DpBirth.SelectedDate == null)
        {
            MessageBox.Show("Выберите дату рождения!");
            return;
        }

        DateTime selectedDate = DpBirth.SelectedDate.Value;

        if (selectedDate > DateTime.Now)
        {
            MessageBox.Show("Автор еще не родился? Выберите дату из прошлого.");
            return;
        }

        if (selectedDate.Year < 800)
        {
            MessageBox.Show("Слишком древний автор. Проверьте год рождения.");
            return;
        }

        // 3. Сохранение данных в объект
        Author.FirstName = TxtFirstName.Text.Trim();
        Author.LastName = TxtLastName.Text.Trim();
        Author.Country = string.IsNullOrWhiteSpace(TxtCountry.Text) ? "Неизвестно" : TxtCountry.Text.Trim();
        Author.BirthDate = selectedDate;

        DialogResult = true;
    }
}