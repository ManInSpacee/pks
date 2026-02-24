using System.Collections.Generic;
using System.Linq;

namespace WpfApp1.Models;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public int PublishYear { get; set; }
    public string ISBN { get; set; } = null!;
    public int QuantityInStock { get; set; }

    // Коллекции для связи многие-ко-многим
    public virtual ICollection<Author> Authors { get; set; } = new List<Author>();
    public virtual ICollection<Genre> Genres { get; set; } = new List<Genre>();

    // Свойства для отображения в DataGrid
    public string AuthorsDisplay => string.Join(", ", Authors.Select(a => a.LastName));
    public string GenresDisplay => string.Join(", ", Genres.Select(g => g.Name));
}