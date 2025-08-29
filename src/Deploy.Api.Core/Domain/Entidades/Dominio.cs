namespace Deploy.Api.Core.Domain.Entidades
{
      public class Dominio
      {
            public int Id { get; set; }
            public int Aplicacao_Id { get; set; }
            public string Subdominio { get; set; }
            public string Dominio_Base { get; set; }
            public string Url_Publica { get; set; }
            public DateTime Criado_Em { get; set; }
      }
}
