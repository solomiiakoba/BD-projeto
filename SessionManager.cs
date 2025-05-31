using System;

namespace App
{
    // Classe para gerenciar a sessão do usuário logado
    public static class SessionManager
    {
        public static int? UsuarioLogadoId { get; private set; }
        public static string NomeUsuario { get; private set; }
        public static string TipoUsuario { get; private set; } // "cliente" ou "artista"
        public static string EmailUsuario { get; private set; }

        public static void IniciarSessao(int usuarioId, string nome, string tipo, string email)
        {
            UsuarioLogadoId = usuarioId;
            NomeUsuario = nome;
            TipoUsuario = tipo;
            EmailUsuario = email;
        }

        public static void EncerrarSessao()
        {
            UsuarioLogadoId = null;
            NomeUsuario = null;
            TipoUsuario = null;
            EmailUsuario = null;
        }

        public static bool IsLogado()
        {
            return UsuarioLogadoId.HasValue;
        }

        public static bool IsCliente()
        {
            return IsLogado() && TipoUsuario == "cliente";
        }

        public static bool IsArtista()
        {
            return IsLogado() && TipoUsuario == "artista";
        }

        public static int GetClienteId()
        {
            if (IsCliente())
            {
                return UsuarioLogadoId.Value;
            }
            throw new InvalidOperationException("Usuário não é um cliente ou não está logado");
        }

        public static int GetArtistaId()
        {
            if (IsArtista())
            {
                return UsuarioLogadoId.Value;
            }
            throw new InvalidOperationException("Usuário não é um artista ou não está logado");
        }
    }
}