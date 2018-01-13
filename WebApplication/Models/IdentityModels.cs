using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace WebApplication.Models
{
    // Sie können Profildaten für den Benutzer durch Hinzufügen weiterer Eigenschaften zur ApplicationUser-Klasse hinzufügen. Weitere Informationen finden Sie unter "http://go.microsoft.com/fwlink/?LinkID=317594".
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Beachten Sie, dass der "authenticationType" mit dem in "CookieAuthenticationOptions.AuthenticationType" definierten Typ übereinstimmen muss.
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Benutzerdefinierte Benutzeransprüche hier hinzufügen
            return userIdentity;
        }

        public virtual ICollection<Enrollment> Enrollments { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public virtual DbSet<Enrollment> Enrollments { get; set; }
        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<Assignment> Assignments { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }
        public virtual DbSet<ContentGroup> ContentGroups { get; set; }
        public virtual DbSet<ContentElement> ContentElements { get; set; }
        public virtual DbSet<Type> Types { get; set; }
    }

    public enum Rating
    {
        Eins, Zwei, Drei, Vier, Fünf
    }

    public class Enrollment
    {
        [Key]
        public int Id { get; set; }
        public DateTime Datetime { get; set; }
        public Rating Rating { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public int CourseId { get; set; }
        public virtual Course Course { get; set; }
    }

    public class Course
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public virtual ApplicationUser Owner { get; set; }
        public virtual ICollection<Enrollment> Enrollments { get; set; }
        public virtual ICollection<Assignment> Assignments { get; set; }
        public virtual ICollection<ContentGroup> ContentGroups { get; set; }
    }

    public class Assignment
    {
        [Key]
        public int Id { get; set; }
        public int CourseId { get; set; }
        public virtual Course Course { get; set; }
        public int TagId { get; set; }
        public virtual Tag Tag { get; set; }
    }

    public class Tag
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Assignment> Assignments { get; set; }
    }

    public class ContentGroup
    {
        [Key]
        public int Id { get; set; }
        public int Order { get; set; }
        public string Header { get; set; }
        public int CourseId { get; set; }
        public virtual Course Course { get; set; }
        public int ContentId { get; set; }
        public virtual ContentElement ContentElement { get; set; }
    }

    public class ContentElement
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; }
        public string URL { get; set; }
        public int Order { get; set; }
        public int TypeId { get; set; }
        public virtual WebApplication.Models.Type Type { get; set; }
        public virtual ICollection<ContentGroup> ContentGroups { get; set; }
    }

    public class Type
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<ContentElement> ContentElements { get; set; }

    }
}