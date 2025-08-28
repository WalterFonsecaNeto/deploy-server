using Deploy.Api.Core.Domain.Response.Base;
using Deploy.Api.Core.Interfaces.Services;
using Deploy.Api.Core.Interfaces.Repositories;
using Deploy.Api.Core.Domain.Entidades;

namespace Deploy.Api.Core.Services
{
      public class ProjetoService : IProjetoService
      {
            private readonly IProjetoRepository _projetoRepository;
            private readonly IUsuarioRepository _usuarioRepository;

            public ProjetoService(IProjetoRepository projetoRepository, IUsuarioRepository usuarioRepository)
            {
                  _projetoRepository = projetoRepository;
                  _usuarioRepository = usuarioRepository;
            }

            #region Métodos Principais
            public async Task<ResponseViewModel<List<Projeto>>> ListarTodosProjetosDeUmUsuariosAsync(int usuarioId)
            {
                  try
                  {
                        if (usuarioId <= 0)
                              return new ResponseViewModel<List<Projeto>>(400, false, new List<string> { "UsuarioId invalido." });

                        var projetos = await _projetoRepository.ListarTodosProjetosDeUmUsuariosAsync(usuarioId);

                        if (projetos.Count() == 0)
                              return new ResponseViewModel<List<Projeto>>(200, true, new List<string> { "Nenhum projeto encontrado para este usuário." });

                        return new ResponseViewModel<List<Projeto>>(200, true, projetos);
                  }
                  catch (Exception ex)
                  {
                        return new ResponseViewModel<List<Projeto>>(500, false, new List<string> { $"Erro ao listar projetos: {ex.Message}" });
                  }

            }

            public async Task<ResponseViewModel<Projeto>> CadastrarProjetoAsync(Projeto projeto)
            {
                  try
                  {
                        var erroValidacao = ValidarProjeto(projeto);

                        if (erroValidacao != null)
                              return erroValidacao;

                        var usuarioExiste = await _usuarioRepository.ObterUsuarioPorIdAsync(projeto.Usuario_Id);
                        
                        if (usuarioExiste == null)
                              return new ResponseViewModel<Projeto>(404, false, new List<string> { "Usuário não encontrado." });

                        projeto.Criado_Em = DateTime.Now;

                        var linhasAfetadas = await _projetoRepository.CadastrarProjetoAsync(projeto);

                        if (linhasAfetadas == 0)
                              return new ResponseViewModel<Projeto>(500, false, new List<string> { "Erro ao criar projeto." });

                        return new ResponseViewModel<Projeto>(201, true, new List<string> { "Projeto criado com sucesso." });

                  }
                  catch (Exception ex)
                  {
                        return new ResponseViewModel<Projeto>(500, false, new List<string> { $"Erro ao cadastrar projeto: {ex.Message}" });
                  }
            }

            #endregion


            #region Validações
            private ResponseViewModel<Projeto> ValidarProjeto(Projeto projeto)
            {
                  if (projeto == null)
                        return new ResponseViewModel<Projeto>(400, false, new List<string> { "Projeto não pode ser nulo." });

                  if (projeto.Usuario_Id <= 0)
                        return new ResponseViewModel<Projeto>(400, false, new List<string> { "Usuario_Id inválido." });

                  if (string.IsNullOrWhiteSpace(projeto.Nome))
                        return new ResponseViewModel<Projeto>(400, false, new List<string> { "Nome do projeto é obrigatório." });

                  if (string.IsNullOrWhiteSpace(projeto.Descricao))
                        return new ResponseViewModel<Projeto>(400, false, new List<string> { "Descrição do projeto é obrigatória." });

                  return null;
            }

            #endregion
      }
}