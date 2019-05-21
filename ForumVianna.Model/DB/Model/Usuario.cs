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
    public class Usuario
    {
        public virtual Guid Id { get; set; }
        [Required(ErrorMessage = "O Nome obrigatório.")]
        [Display(Name = "Nome do Usuário")]        
        public virtual String Nome {get; set; }

        [Required(ErrorMessage = "O Apelido é obrigatório.")]        
        [Display(Name = "Nickname")]
        public virtual String Apelido { get; set; }

        [Required(ErrorMessage = "O E-mail é obrigatório.")]
        [Display(Name = "E-mail")]
        [DataType(DataType.EmailAddress, ErrorMessage = "E-mail incorreto.")]
        public virtual String Email { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date)]
        public virtual DateTime DtNascimento { get; set; }

        [Required(ErrorMessage = "A Senha é obrigatória.")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public virtual String Senha { get; set; }
        public virtual String Foto { get; set; }
        public virtual Boolean Ativo { get; set; }

        [Required(ErrorMessage = "A Confirmação de Senha é obrigatória.")]
        [Compare("Senha", ErrorMessage = "Confirmação não compatível com a senha.")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public virtual String ConfirmarSenha { get; set; }

        public virtual IList<Topico> Topicos { get; set; }
        public virtual IList<Discussao> Discussoes { get; set; }
        public virtual IList<Comentario> Comentarios { get; set; }

        public Usuario()
        {
            Topicos = new List<Topico>();
            Discussoes = new List<Discussao>();
            Comentarios = new List<Comentario>();
        }
    }

    public class UsuarioMap : ClassMapping<Usuario>
    {
        public UsuarioMap()
        {
            Id(x => x.Id, m => m.Generator(Generators.Guid));

            Property(x => x.Nome);
            Property(x => x.Apelido);
            Property(x => x.Email);
            Property(x => x.DtNascimento, m => m.Column(c => c.SqlType("date")));
            Property(x => x.Senha);
            Property(x => x.Foto);
            Property(x => x.Ativo, m => m.Column(c => c.Default(1)));

            Bag(x => x.Topicos, m =>
                {
                    m.Inverse(true);
                    m.Cascade(Cascade.DeleteOrphans);
                    m.Lazy(CollectionLazy.Lazy);
                    m.Key(k => k.Column("idUsuario"));
                }, 
                o => o.OneToMany());

            Bag(x => x.Discussoes, m =>
                {
                    m.Inverse(true);
                    m.Cascade(Cascade.DeleteOrphans);
                    m.Lazy(CollectionLazy.Lazy);
                    m.Key(k => k.Column("idUsuario"));
                },
                o => o.OneToMany());

            Bag(x => x.Comentarios, m =>
                {
                    m.Inverse(true);
                    m.Cascade(Cascade.DeleteOrphans);
                    m.Lazy(CollectionLazy.Lazy);
                    m.Key(k => k.Column("idUsuario"));
                },
                o => o.OneToMany());
        }
    }
}
