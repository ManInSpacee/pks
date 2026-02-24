using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using WpfApp1.Data;
using WpfApp1.Models;

namespace WpfApp1;

public partial class MainWindow : Window
{
    private LibraryContext _context = new LibraryContext();

    public MainWindow()
    {
        InitializeComponent();
        
        _context.Database.EnsureDeleted(); 
        
        _context.Database.EnsureCreated();
        SeedData();
        LoadData();
    }

    private void SeedData()
    {
        
        var classic = new Genre { Name = "Классика", Description = "Золотой фонд литературы" };
        var sciFi = new Genre { Name = "Фантастика", Description = "Космос и технологии" };
        var detective = new Genre { Name = "Детектив", Description = "Интриги и расследования" };
        var dystopia = new Genre { Name = "Антиутопия", Description = "Мир будущего" };
        
        _context.Genres.AddRange(classic, sciFi, detective, dystopia);

        // 2. Авторы
        var bulgakov = new Author { FirstName = "Михаил", LastName = "Булгаков", Country = "СССР", BirthDate = new DateTime(1891, 5, 15) };
        var orwell = new Author { FirstName = "Джордж", LastName = "Оруэлл", Country = "Великобритания", BirthDate = new DateTime(1903, 6, 25) };
        var bradbury = new Author { FirstName = "Рэй", LastName = "Брэдбери", Country = "США", BirthDate = new DateTime(1920, 8, 22) };
        var doyle = new Author { FirstName = "Артур", LastName = "Конан Дойл", Country = "Великобритания", BirthDate = new DateTime(1859, 5, 22) };

        _context.Authors.AddRange(bulgakov, orwell, bradbury, doyle);
        _context.SaveChanges(); 

        var books = new List<Book>();

        var b1 = new Book { Title = "Мастер и Маргарита", PublishYear = 1967, ISBN = "9785170881658" };
        b1.Authors.Add(bulgakov);
        b1.Genres.Add(classic);
        books.Add(b1);

        var b2 = new Book { Title = "1984", PublishYear = 1949, ISBN = "9785170801151" };
        b2.Authors.Add(orwell);
        b2.Genres.Add(dystopia);
        books.Add(b2);

        var b3 = new Book { Title = "451 градус по Фаренгейту", PublishYear = 1953, ISBN = "9785170942281" };
        bradbury.Books.Add(b3); 
        b3.Genres.Add(sciFi);
        b3.Genres.Add(dystopia);
        books.Add(b3);

        var b4 = new Book { Title = "Этюд в багровых тонах", PublishYear = 1887, ISBN = "9785699450374" };
        b4.Authors.Add(doyle);
        b4.Genres.Add(detective);
        books.Add(b4);

        _context.Books.AddRange(books);
        _context.SaveChanges();
    }

    private void LoadData()
    {
        var books = _context.Books
            .Include(b => b.Authors)
            .Include(b => b.Genres)
            .ToList();

        DgBooks.ItemsSource = books;
        ComboGenre.ItemsSource = _context.Genres.ToList();
        ComboAuthor.ItemsSource = _context.Authors.ToList();
    }

    private void ApplyFilters(object sender, EventArgs e)
    {
        if (_context == null || TxtSearch == null || ComboGenre == null || ComboAuthor == null) 
            return;

        var allBooks = _context.Books
            .Include(b => b.Authors)
            .Include(b => b.Genres)
            .ToList(); 

        var filtered = allBooks.AsEnumerable();

        string search = TxtSearch.Text.Trim().ToLower();
        if (!string.IsNullOrWhiteSpace(search))
        {
            filtered = filtered.Where(b => 
                (b.Title != null && b.Title.ToLower().Contains(search)) || 
                (b.ISBN != null && b.ISBN.Contains(search))
            );
        }

        if (ComboGenre.SelectedItem is Genre g)
        {
            filtered = filtered.Where(b => b.Genres.Any(genre => genre.Id == g.Id));
        }

        if (ComboAuthor.SelectedItem is Author a)
        {
            filtered = filtered.Where(b => b.Authors.Any(author => author.Id == a.Id));
        }

        var resultList = filtered.ToList();
        DgBooks.ItemsSource = resultList;

        this.Title = $"Библиотека (Найдено книг: {resultList.Count})";
    }

    private void BtnRefresh_Click(object sender, RoutedEventArgs e)
    {
        TxtSearch.Text = "";
        ComboGenre.SelectedIndex = -1;
        ComboAuthor.SelectedIndex = -1;
        LoadData();
    }

    private void BtnAdd_Click(object sender, RoutedEventArgs e)
    {
        var win = new BookWindow(null, _context) { Owner = this };
        if (win.ShowDialog() == true) { _context.Books.Add(win.Book); _context.SaveChanges(); LoadData(); }
    }

    private void BtnEdit_Click(object sender, RoutedEventArgs e)
    {
        if (DgBooks.SelectedItem is Book selected)
        {
            var bToEdit = _context.Books.Include(b => b.Authors).Include(b => b.Genres).First(b => b.Id == selected.Id);
            var win = new BookWindow(bToEdit, _context) { Owner = this };
            if (win.ShowDialog() == true) { _context.SaveChanges(); LoadData(); }
        }
    }

    private void BtnDelete_Click(object sender, RoutedEventArgs e)
    {
        if (DgBooks.SelectedItem is Book s && MessageBox.Show($"Удалить {s.Title}?", "??", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        { _context.Books.Remove(s); _context.SaveChanges(); LoadData(); }
    }

    private void BtnManageAuthors_Click(object sender, RoutedEventArgs e)
    {
        var win = new AuthorWindow(null, _context) { Owner = this };
        if (win.ShowDialog() == true) { _context.Authors.Add(win.Author); _context.SaveChanges(); LoadData(); }
    }

    private void BtnEditAuthor_Click(object sender, RoutedEventArgs e)
    {
        if (ComboAuthor.SelectedItem is Author selected)
        {
            var win = new AuthorWindow(selected, _context) { Owner = this };
            if (win.ShowDialog() == true) { _context.SaveChanges(); LoadData(); }
        }
    }
}