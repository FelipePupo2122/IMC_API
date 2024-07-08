using System.ComponentModel.DataAnnotations;

namespace IMCCalculator.Models
{
    public class Imc
    {
        public Imc(double altura, double peso, string usuarioId)
        {
            Id = Guid.NewGuid().ToString();
            Altura = altura;
            Peso = peso;
            UsuarioId = usuarioId;
            CalcularResultado();
        }

        public string Id { get; set; }

        [Required(ErrorMessage = "O campo Altura é obrigatório.")]
        public double Altura { get; set; }

        [Required(ErrorMessage = "O campo Peso é obrigatório.")]
        public double Peso { get; set; }

        public double Resultado { get; private set; }

        public string Classificacao { get; private set; } = string.Empty;

        public string UsuarioId { get; set; }

        public void CalcularResultado()
        {
            Resultado = Peso / (Altura * Altura);
            Classificacao = ObterClassificacao(Resultado);
        }

        private string ObterClassificacao(double imc)
        {
            if (imc < 18.5) return "Magreza";
            if (imc >= 18.5 && imc <= 24.9) return "Normal";
            if (imc >= 25.0 && imc <= 29.9) return "Sobrepeso";
            if (imc >= 30.0 && imc <= 39.9) return "Obesidade";
            if (imc >= 40.0) return "Obesidade Grave";
            return "Indeterminado";
        }
    }
}
