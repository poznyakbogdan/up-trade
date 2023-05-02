using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL;

[Table("Wallets")]
public class Wallet
{
    [Key]
    public int Id { get; set; }
    public string Address { get; set; }
}