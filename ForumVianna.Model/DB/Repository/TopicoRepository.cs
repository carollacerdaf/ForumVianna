using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForumVianna.Model.DB.Model;
using NHibernate;

namespace ForumVianna.Model.DB.Repository
{
    public class TopicoRepository : RepositoryBase<Topico>
    {
        public TopicoRepository(ISession session) : base(session)
        {
        }

        public IList<Topico> BuscarPelaSituacao(Boolean ativo)
        {
            return Session.Query<Topico>().Where(w => w.Ativo == ativo).ToList();
        }
    }
}
