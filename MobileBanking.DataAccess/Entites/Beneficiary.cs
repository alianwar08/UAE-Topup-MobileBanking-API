using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileBanking.DataAccess.Entities
{
    public class Beneficiary
    {
        [Key]
        public int BeneficiaryID { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; }

        [Required]
        [MaxLength(20)]
        public string Nickname { get; set; }

        [Required]
        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        //public virtual User User { get; set; }

        //public virtual ICollection<TopUpTransaction> TopUpTransactions { get; set; }
    }
}
