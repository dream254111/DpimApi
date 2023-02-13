using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DpimProject.Models.Data.DataModels
{
    [Table("document_type")]
    public class document_type //Create 2/28/2021 7:44:14 AM
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? id { get; set; }

        [MaxLength(510)]
        public string document_name { get; set; }

        [MaxLength(1)]
        public string is_active { get; set; }

        public int? is_delete { get; set; }

        [MaxLength(10)]
        public string create_by { get; set; }

        public DateTime? create_dt { get; set; }

        [MaxLength(10)]
        public string update_by { get; set; }

        public DateTime? update_dt { get; set; }

    }
}