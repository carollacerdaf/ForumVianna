using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForumVianna.Model.DB.Model;
using NHibernate;

namespace ForumVianna.Model.DB.Repository
{
    public class ComentarioRepository : RepositoryBase<Comentario>
    {
        public ComentarioRepository(ISession session) : base(session)
        {
        }

        public IList<Comentario> BuscarPeloIdDiscussao(Guid id)
        {
            return Session.Query<Comentario>().Where(w => w.Discussao.Id == id).OrderBy(o => o.Criacao).ToList();
        }
    }
}
