using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using WpfApp1.Data;
using WpfApp1.Models;

namespace WpfApp1;

public partial class BookWindow : Window
{
    public Book Book { get; set; }
    private LibraryContext _db;

    public BookWindow(Book? b, LibraryContext context)
    {
        InitializeComponent();
        _db = context;
        ListAuthors.ItemsSource = _db.Authors.ToList();
        ListGenres.ItemsSource = _db.Genres.ToList();

        if (b != null)
        {
            Book = b;
            TxtTitle.Text = b.Title;
            TxtYear.Text = b.PublishYear.ToString();
            TxtIsbn.Text = b.ISBN;
            foreach (var a in b.Authors) ListAuthors.SelectedItems.Add(ListAuthors.Items.Cast<Author>().First(x => x.Id == a.Id));
            foreach (var g in b.Genres) ListGenres.SelectedItems.Add(ListGenres.Items.Cast<Genre>().First(x => x.Id == g.Id));
        }
        else Book = new Book();
    }

    // Блокируем ввод букв в реальном времени
    private void OnlyNumbers(object sender, TextCompositionEventArgs e)
    {
        e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
    }

    private void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TxtTitle.Text)) { MessageBox.Show("Введите название!"); return; }
        
        if (!int.TryParse(TxtYear.Text, out int year) || year < 868 || year > 2026)
        { MessageBox.Show("Введите корректный год!"); return; }

        string isbn = TxtIsbn.Text.Trim();
        if (isbn.Length != 10 && isbn.Length != 13)
        {
            MessageBox.Show("ISBN должен быть 10 или 13 цифр!");
            return;
        }

        if (ListAuthors.SelectedItems.Count == 0 || ListGenres.SelectedItems.Count == 0)
        { MessageBox.Show("Выберите автора и жанр!"); return; }

        Book.Title = TxtTitle.Text;
        Book.PublishYear = year;
        Book.ISBN = isbn;

        Book.Authors.Clear();
        foreach (Author a in ListAuthors.SelectedItems) Book.Authors.Add(a);
        Book.Genres.Clear();
        foreach (Genre g in ListGenres.SelectedItems) Book.Genres.Add(g);

        DialogResult = true;
    }
}