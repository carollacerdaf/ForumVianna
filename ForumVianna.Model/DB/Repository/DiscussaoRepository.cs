using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForumVianna.Model.DB.Model;
using NHibernate;

namespace ForumVianna.Model.DB.Repository
{
    public class DiscussaoRepository : RepositoryBase<Discussao>
    {
        public DiscussaoRepository(ISession session) : base(session)
        {
        }

        public IList<Discussao> BuscarPelaSituacao(Boolean ativo)
        {
            return Session.Query<Discussao>().Where(w => w.Ativo == ativo).ToList();
        }

        public Discussao BuscarPorDono(Guid Id)
        {
            try
            {
                var disc = Session.Query<Discussao>()
                    .FirstOrDefault(f => f.Dono.Id == Id);
                return disc;
            }
            catch (Exception ex)
            {
                throw new Exception("Não foi possível obter usuário", ex);
            }
        }
    }
}
