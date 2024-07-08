using System.ComponentModel.DataAnnotations;

namespace IMCCalculator.Models
{
    public class Usuario
    {
        public Usuario(string nome, DateTime dataDeNascimento)
        {
            Id = Guid.NewGuid().ToString();
            Nome = nome;
            DataDeNascimento = dataDeNascimento;
        }

        public string Id { get; set; }

        [Required(ErrorMessage = "O campo Nome é obrigatório.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O campo Data de Nascimento é obrigatório.")]
        public DateTime DataDeNascimento { get; set; }
    }
}
