using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MZ.Domain.Interface;

namespace MZ.Domain.Entities
{
    [Table("Language")]
    public class LanguageEntity : ILanguage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string DisplayName { get; set; } = string.Empty;

        public string CultureCode { get; set; } = string.Empty;

    }
}
