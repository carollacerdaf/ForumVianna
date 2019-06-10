using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForumVianna.Model.DB.Model;
using NHibernate;

namespace ForumVianna.Model.DB.Repository
{
    public class UsuarioRepository : RepositoryBase<Usuario>
    {
        public UsuarioRepository(ISession session) : base(session)
        {
        }

        public Usuario obterUsuario(String login, String senha)
        {
            try
            {
                var usuario = Session.Query<Usuario>()
                    .FirstOrDefault(f =>
                        f.Senha == senha &&
                        (f.Apelido == login || f.Email == login));
                return usuario;
            }
            catch (Exception ex)
            {
                throw new Exception("Não foi possível obter usuário", ex);
            }
        }
    }
}
