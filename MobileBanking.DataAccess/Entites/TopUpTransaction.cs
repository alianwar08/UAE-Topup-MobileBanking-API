using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileBanking.DataAccess.Entities
{
    public class TopUpTransaction
    {
        [Key]
        public int TransactionID { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; }

        [ForeignKey("Beneficiary")]
        public int BeneficiaryID { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        public DateTime TransactionDate { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Fee { get; set; }

        public DateTime? UpdatedAt { get; set; }

        //public virtual User User { get; set; }

        //public virtual Beneficiary Beneficiary { get; set; }
    }
}
