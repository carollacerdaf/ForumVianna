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
    public class Topico
    {
        public virtual Guid Id { get; set; }

        [Required(ErrorMessage = "O Título do tópico é obrigatório.")]
        [Display(Name = "Título")]
        public virtual String Titulo { get; set; }

        [Required(ErrorMessage = "A Descrição da discussão é obrigatório.")]
        [Display(Name = "Descrição")]
        [DataType(DataType.MultilineText)]
        public virtual String Descricao { get; set; }
        public virtual Boolean Ativo { get; set; }

        public virtual Usuario Dono { get; set; }
        public virtual IList<Discussao> Discussoes { get; set; }

        public Topico()
        {
            Discussoes = new List<Discussao>();
        }
    }

    public class TopicoMap : ClassMapping<Topico>
    {
        public TopicoMap()
        {
            Id(x => x.Id, m => m.Generator(Generators.Guid));

            Property(x => x.Titulo);
            Property(x => x.Descricao);
            Property(x => x.Ativo, m => m.Column(x => x.Default(1)));

            ManyToOne<Usuario>(x => x.Dono, m =>
            {
                m.Column("idUsuario");
            });

            Bag(x => x.Discussoes, m =>
                {
                    m.Inverse(true);
                    m.Cascade(Cascade.DeleteOrphans);
                    m.Lazy(CollectionLazy.Lazy);
                    m.Key(k => k.Column("idTopico"));
                },
                o => o.OneToMany()
            );
        }
    }
}
