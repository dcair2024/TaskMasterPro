using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace TaskMasterPro__v3._0_.Models
{
    public class Tarefa
    {
        public int Id { get; set; }

        [Required]
        public string Titulo { get; set; }

        public string? Descricao { get; set; }

        [DataType(DataType.Date)]
        public DateTime Data { get; set; }

        public bool Concluida { get; set; }

        // Propriedades de usuário
        public string? UserId { get; set; } // deve ser anulável
        public ApplicationUser? User { get; set; } // deve ser anulável
    }

}
