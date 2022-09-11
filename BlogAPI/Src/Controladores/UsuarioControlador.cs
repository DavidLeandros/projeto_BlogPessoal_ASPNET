using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogAPI.Src.Modelos;
using BlogAPI.Src.Repositorios;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.Src.Controladores
{
    [ApiController]
    [Route("api/Usuarios")]
    [Produces("application/json")]
    public class UsuarioControlador : ControllerBase
    {
        #region Atributos

        private readonly IUsuario _repositorio;
        private readonly IAutenticacao _servicos;

        #endregion


        #region Construtores

        public UsuarioControlador(IUsuario repositorio, IAutenticacao servicos)
        {
            _repositorio = repositorio;
            _servicos = servicos;
        }

        #endregion


        #region Métodos

        [HttpGet]
        public async Task<ActionResult> PegarTodosUsuariosAync()
        {
            var lista = await _repositorio.PegarTodosUsuariosAsync();

            if (lista.Count < 1) return NoContent();

            return Ok(lista);
        }

        [HttpPost("cadastrar")]
        public async Task<ActionResult> NovoUsuarioAsync([FromBody] Usuario usuario)
        {
            try
            {
                await _repositorio.NovoUsuarioAsync(usuario);
                return Created($"api/Usuarios/{usuario.Email}", usuario);
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }
        
        [HttpPost("logar")]
        [AllowAnonymous]
        public async Task<ActionResult> LogarAsync([FromBody] Usuario usuario)
        {
            var auxiliar = await _repositorio.PegarUsuarioPeloEmailAsync(usuario.Email);

            if (auxiliar == null) return Unauthorized(new { Mensagem = "E-mail invalido" });

            if (auxiliar.Senha != _servicos.codificarSenha(usuario.Senha))
                return Unauthorized(new { Mensagem = "Senha invalida" });

            var token = "Bearer" + _servicos.GerarToken(auxiliar);
            return Ok(new { usuario = auxiliar, Token = token });
        }

        [HttpGet("email/{emailUsuario}")]
        public async Task<ActionResult> PegarUsuarioPeloEmailAsync([FromRoute] string
        emailUsuario)
        {
            var usuario = await _repositorio.PegarUsuarioPeloEmailAsync(emailUsuario);
            if (usuario == null) return NotFound(new
            {
                Mensagem = "Usuario não encontrado" });
            return Ok(usuario);
        }

        #endregion
    }

}