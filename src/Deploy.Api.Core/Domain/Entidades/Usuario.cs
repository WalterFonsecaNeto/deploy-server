namespace Deploy.Api.Core.Domain.Entidades{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public DateTime Criado_Em { get; set; }

    }
}
