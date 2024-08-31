using Microsoft.EntityFrameworkCore;

namespace ContactBook.Models;

public partial class ContactsContext : DbContext
{
    public ContactsContext()
    {
    }

    public ContactsContext(DbContextOptions<ContactsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Contact> Contacts { get; set; }

    public virtual DbSet<PhoneCode> PhoneCodes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning 
        => optionsBuilder.UseSqlite("Data Source=contacts.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contact>(entity =>
        {
            entity.ToTable("contacts");
            entity.Ignore(e => e.CanEdit);
            entity.Ignore(e => e.CanDelete);

            entity.HasIndex(e => e.Email, "IX_contacts_Email").IsUnique();

            entity.HasIndex(e => e.PhoneNumber, "IX_contacts_PhoneNumber").IsUnique();
        });

        modelBuilder.Entity<PhoneCode>(entity =>
        {
            entity.HasKey(e => e.CodeId);

            entity.ToTable("phoneCodes");

            entity.HasIndex(e => e.Code, "IX_phoneCodes_Code").IsUnique();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
