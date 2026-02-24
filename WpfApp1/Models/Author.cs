using System;
using System.Collections.Generic;

namespace WpfApp1.Models;

public class Author
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTime BirthDate { get; set; }
    public string Country { get; set; } = null!;
    
    public virtual ICollection<Book> Books { get; set; } = new List<Book>();

    // Для удобства отображения в списках
    public string FullName => $"{LastName} {FirstName}";
}