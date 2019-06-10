using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace ForumVianna.Model.DB.Model
{
    public class Comentario
    {
        public virtual Guid Id { get; set; }
        [Required(ErrorMessage = "Comentário é obrigatório.")]
        public virtual String Texto { get; set; }
        public virtual DateTime Criacao { get; set; }
        public virtual String Arquivo { get; set; }

        public virtual Usuario Dono { get; set; }
        public virtual Discussao Discussao { get; set; }}

    public class ComentarioMap : ClassMapping<Comentario>
    {
        public ComentarioMap()
        {
            Id(x => x.Id, m => m.Generator(Generators.Guid));

            Property(x => x.Texto);
            Property(x => x.Criacao);
            Property(x => x.Arquivo);

            ManyToOne(x => x.Dono, m =>
            {
                m.Column("idUsuario");
            });

            ManyToOne(x => x.Discussao, m =>
            {
                m.Column("idDiscussao");
            });
        }
    }
}
