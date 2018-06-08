using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClickEnglish
{
    class DictionaryContext : DbContext
    {
        public DictionaryContext() : base("Dictionary")
        {
            //DropCreateDatabaseIfModelChanges
            //DropCreateDatabaseAlways
            Database.SetInitializer<DictionaryContext>(new DropCreateDatabaseIfModelChanges<DictionaryContext>());
        }

        public DbSet<Word> Dictionary { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<UserSettings> UserSettings { get; set; }
    }

    public class Word : IQuestion
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string English { get; set; }
        [Required]
        public string Polish { get; set; }

        //Foreign key to Category
        public int CategoryID { get; set; }
        [Required]
        [Index("IX_FirstAndSecond", 1, IsUnique = true)]
        public Category Category { get; set; }
        public double Difficulty { get; set; }
        public byte[] Picture { get; set; }
    }

    public class Category
    {
        [Key]
        public int CategoryID { get; set; }
        public string Title { get; set; }

        public ICollection<Word> Words { get; set; }

        public override string ToString()
        {
            return Title;
        }

    }

    public class UserSettings
    {
        [Key]
        public int ID { get; set; }
        public bool Sound { get; set; }
        public int TimeChallange { get; set; }
        public int VocabularySize { get; set; }
    }
}
