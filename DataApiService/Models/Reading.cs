using System.ComponentModel.DataAnnotations.Schema;

namespace DataApiService.Models
{
    public class Reading
    {
        public int Id { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime TimeStamp { get; set; }
        public Double Value { get; set; }
    }
}
