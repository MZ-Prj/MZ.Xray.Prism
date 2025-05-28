using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MZ.Domain.Entities
{
    [Table("AppSetting")]
    public class AppSettingEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public bool IsUsernameSave { get; set; }
        public string LastestUsername { get; set; }
    }
}
