using Deploy.Api.Core.Domain.Entidades;
using Deploy.Api.Core.Domain.Response.Base;
using Deploy.Api.Core.Interfaces.Repositories;
using Deploy.Api.Core.Interfaces.Services;

namespace Deploy.Api.Core.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        public UsuarioService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        #region Métodos Principais
        public async Task<ResponseViewModel<Usuario>> ValidarLoginUsuarioAsync(Usuario usuarioLogin)
        {
            try
            {
                var erroValidacao = ValidarUsuarioExistenteParaRealizarLogin(usuarioLogin);

                if (erroValidacao != null)
                    return erroValidacao;
               
                var usuarioExistente = await _usuarioRepository.ObterUsuarioPorEmailAsync(usuarioLogin.Email);

                if (usuarioExistente == null)
                    return new ResponseViewModel<Usuario>(401, false, new List<string> { "E-mail ou senha incorretos" });

                if (!CompararSenha(usuarioLogin.Senha, usuarioExistente.Senha))
                    return new ResponseViewModel<Usuario>(401, false, new List<string> { "E-mail ou senha incorretos" });


                return new ResponseViewModel<Usuario>(200, true, usuarioExistente);
            }
            catch (Exception ex)
            {
                return new ResponseViewModel<Usuario>(500, false, new List<string> { $"Erro ao validar login do usuário: {ex.Message}" });
            }
        }
        public async Task<ResponseViewModel<List<Usuario>>> ListarTodosUsuariosAsync()
        {
            try
            {
                var usuarios = await _usuarioRepository.ListarTodosUsuariosAsync();
                return new ResponseViewModel<List<Usuario>>(200, true, usuarios);
            }
            catch (Exception ex)
            {
                return new ResponseViewModel<List<Usuario>>(500, false, new List<string> { $"Erro ao listar usuários: {ex.Message}" });
            }
        }  
        public async Task<ResponseViewModel<Usuario>> CadastrarUsuarioAsync(Usuario usuario)
        {
            try
            {
                var erroValidacao = ValidarUsuario(usuario);

                if (erroValidacao != null)
                    return erroValidacao;

                erroValidacao = await ValidarEmailExistente(usuario.Email);

                if (erroValidacao != null)
                    return erroValidacao;

                erroValidacao = ValidarSenha(usuario.Senha);

                if (erroValidacao != null)
                    return erroValidacao;

                usuario.Senha = CriptografarSenha(usuario.Senha);

                usuario.Criado_Em = DateTime.Now;

                var linhasAfetadas = await _usuarioRepository.InserirUsuarioAsync(usuario);

                if (linhasAfetadas == 0)
                    return new ResponseViewModel<Usuario>(500, false, new List<string> { "Erro ao criar usuário." });

                return new ResponseViewModel<Usuario>(201, true, new List<string> { "Usuário criado com sucesso." });
            }
            catch (Exception ex)
            {
                return new ResponseViewModel<Usuario>(500, false, new List<string> { $"Erro ao criar usuário: {ex.Message}" });
            }
        }

        #endregion


        #region Validações
        private ResponseViewModel<Usuario> ValidarUsuario(Usuario usuario)
        {
            if (usuario == null)
                return new ResponseViewModel<Usuario>(400, false, new List<string> { "Usuário não pode ser nulo." });

            if (string.IsNullOrEmpty(usuario.Nome))
                return new ResponseViewModel<Usuario>(400, false, new List<string> { "Nome é obrigatório." });

            return null;
        }
        private async Task<ResponseViewModel<Usuario>> ValidarEmailExistente(string email)
        {

            if (string.IsNullOrEmpty(email))
                return new ResponseViewModel<Usuario>(400, false, new List<string> { "Email é obrigatório." });

            var usuarioExistente = await _usuarioRepository.ObterUsuarioPorEmailAsync(email);

            if (usuarioExistente != null)
                return new ResponseViewModel<Usuario>(400, false, new List<string> { "Email já cadastrado." });

            return null;
        }
        private ResponseViewModel<Usuario> ValidarSenha(string senha)
        {
            if (string.IsNullOrEmpty(senha))
                return new ResponseViewModel<Usuario>(400, false, new List<string> { "Senha é obrigatória." });

            if (senha.Length < 6)
                return new ResponseViewModel<Usuario>(400, false, new List<string> { "Senha deve ter pelo menos 6 caracteres." });

            if (senha.Length > 20)
                return new ResponseViewModel<Usuario>(400, false, new List<string> { "Senha deve ter no máximo 20 caracteres." });

            if (senha.Contains(" "))
                return new ResponseViewModel<Usuario>(400, false, new List<string> { "Senha não pode conter espaços." });

            return null;
        }
        private ResponseViewModel<Usuario> ValidarUsuarioExistenteParaRealizarLogin(Usuario usuario)
        {
            if (usuario == null)
                return new ResponseViewModel<Usuario>(400, false, new List<string> { "Usuário não pode ser nulo." });

            if (string.IsNullOrEmpty(usuario.Email))
                return new ResponseViewModel<Usuario>(400, false, new List<string> { "Email é obrigatório." });

            if (string.IsNullOrEmpty(usuario.Senha))
                return new ResponseViewModel<Usuario>(400, false, new List<string> { "Senha é obrigatória." });

            return null;
        }
        #endregion


        #region Uteis
        private string CriptografarSenha(string senha)
        {
            return BCrypt.Net.BCrypt.HashPassword(senha);
        }
        private bool CompararSenha(string senhaDigitada, string senhaHash)
        {
            return BCrypt.Net.BCrypt.Verify(senhaDigitada, senhaHash);
        }
        #endregion

    }

}