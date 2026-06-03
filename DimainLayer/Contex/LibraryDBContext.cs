using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DimainLayer.Model;
using Microsoft.EntityFrameworkCore;

namespace DimainLayer.Contex
{
    public class LibraryDBContext:DbContext
    {
        public LibraryDBContext(DbContextOptions<LibraryDBContext> options):base(options) { }
       
        public DbSet<User> Users { get; set; }
        public DbSet<MembershipCard> MembershipCards { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //---User
            modelBuilder.Entity<User>().HasIndex(u => u.MembershipNumber).IsUnique();
            modelBuilder.Entity<User>().Property(u => u.Status).HasConversion<string>();
            modelBuilder.Entity<User>().HasOne(u=>u.MembershipCard).WithOne(m=>m.User).HasForeignKey<MembershipCard>(m=>m.UserId);
            modelBuilder.Entity<User>().HasMany(u => u.Reservation).WithOne(r=>r.User).HasForeignKey(r=>r.UserId);

            //MembershipCard
            modelBuilder.Entity<MembershipCard>().Property(m=>m.Status).HasConversion<string>();

            //Book
            modelBuilder.Entity<Book>().HasIndex(b=>b.ISBN).IsUnique(); 
            modelBuilder.Entity<Book>().Property(b=>b.BookStatus).HasConversion<string>();
            modelBuilder.Entity<Book>().HasMany(b=>b.Reservations).WithOne(r=>r.Book).HasForeignKey(r=>r.BookId);

            //---Category
            modelBuilder.Entity<Category>().HasMany(c=>c.Books).WithOne(b=>b.Category).HasForeignKey(b=>b.CategoryId);

            //Reservation
            modelBuilder.Entity<Reservation>().Property(r=>r.ReservationStatus).HasConversion<string>();

        }
    }
}
