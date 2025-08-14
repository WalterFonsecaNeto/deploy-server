namespace Deploy.Api.Core.Domain.Response.Base
{
    public class ResponseViewModel<T>
    {
        public int StatusCode { get; set; }
        public bool Sucesso { get; set; }
        public T? Dados { get; set; }  
        public List<string>? Notificacoes { get; set; } 

        // Construtor para respostas COM DADOS (notificações vazias)
        public ResponseViewModel(int statusCode, bool sucesso, T dados)
        {
            StatusCode = statusCode;
            Sucesso = sucesso;
            Dados = dados;
            Notificacoes = null;  
        }

        // Construtor para respostas COM NOTIFICAÇÕES (dados nulos)
        public ResponseViewModel(int statusCode, bool sucesso, List<string> notificacoes)
        {
            StatusCode = statusCode;
            Sucesso = sucesso;
            Dados = default;  
            Notificacoes = notificacoes;
        }

    }
}