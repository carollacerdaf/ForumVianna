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
    public class Discussao
    {
        public virtual Guid Id { get; set; }
        [Required(ErrorMessage = "O Título da discussão é obrigatório.")]
        [Display(Name = "Título")]
        public virtual String Titulo { get; set; }

        [Required(ErrorMessage = "A Descrição da discussão é obrigatório.")]
        [Display(Name = "Descrição")]
        [DataType(DataType.MultilineText)]
        public virtual String Descricao { get; set; }

        public virtual DateTime Criacao { get; set; }
        public virtual Boolean Ativo { get; set; }
        [Required(ErrorMessage = "O campo Situação é obrigatório")]
        public virtual String Situacao { get; set; }
        
        public virtual Topico Topico { get; set; }
        public virtual Usuario Dono { get; set; }

        public virtual IList<Comentario> Comentarios { get; set; }

        public Discussao()
        {
            Comentarios = new List<Comentario>();
        }
    }

    public class DiscussaoMap : ClassMapping<Discussao>
    {
        public DiscussaoMap()
        {
            Id(x => x.Id, m => m.Generator(Generators.Guid));

            Property(x => x.Titulo);
            Property(x => x.Descricao);
            Property(x => x.Criacao);
            Property(x => x.Situacao);
            Property(x => x.Ativo, m => m.Column(c => c.Default(1)));

            ManyToOne(x => x.Topico, m =>
            {
                m.Column("idTopico");
            });

            ManyToOne(x => x.Dono, m =>
            {
                m.Column("idUsuario");
            });

            Bag(x => x.Comentarios, m =>
                {
                    m.Key(k => k.Column("idDiscussao"));
                    m.Inverse(true);
                    m.Cascade(Cascade.DeleteOrphans);
                    m.Lazy(CollectionLazy.Lazy);
                },
                o => o.OneToMany());
        }
    }
}
