namespace Deploy.Api.Core.Domain.Entidades{
    public class Projeto
    {
        public int Id { get; set; }
        public int Usuario_Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public DateTime Criado_Em { get; set; }
    }
}
