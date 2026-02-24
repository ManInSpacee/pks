using System.Collections.Generic;

namespace WpfApp1.Models;

public class Genre
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}