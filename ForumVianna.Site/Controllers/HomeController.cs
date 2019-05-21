using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ForumVianna.Model.DB;
using ForumVianna.Model.DB.Model;
using ForumVianna.Site.Utils;

namespace ForumVianna.Site.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (LoginUtils.Instance.Usuario != null)
            {
                return RedirectToAction("Discussoes");
            }

            return View();
        }

        public ActionResult Usuarios()
        {
            var usuarios = DbFactory.Instance.UsuarioRepository.FindAll();

            return View(usuarios);
        }

        public ActionResult AddUsuario()
        {
            var usuario = new Usuario()
            {
                Ativo = true
            };

            return View("AddEdtUsuario", usuario);
        }

        public ActionResult EdtUsuario(Guid id)
        {
            var usuario = DbFactory.Instance.UsuarioRepository.FindFirstById(id);
            if(usuario != null)
                return View("AddEdtUsuario", usuario);

            return RedirectToAction("Usuarios");
        }

        public ActionResult DelUsuario(Guid id)
        {
            var usuario = DbFactory.Instance.UsuarioRepository.FindFirstById(id);
            if (usuario != null)
            {
                usuario.Ativo = false;
                DbFactory.Instance.UsuarioRepository.Update(usuario);
            }

            return RedirectToAction("Usuarios");
        }

        public ActionResult GravarUsuario(Usuario usuario)
        {
            usuario.Ativo = true;
            usuario = DbFactory.Instance.UsuarioRepository.SaveOrUpdate(usuario);

            LoginUtils.Instance.Logar(usuario.Apelido, usuario.Senha);

            return RedirectToAction("Usuarios");
        }

        public ActionResult Topicos()
        {
            var topicos = DbFactory.Instance.TopicoRepository.BuscarPelaSituacao(true);

            return View(topicos);
        }

        public ActionResult AddTopico()
        {
            var topico = new Topico()
            {
                Dono = LoginUtils.Instance.Usuario
            };

            return View(topico);
        }

        public ActionResult GravarTopico(Topico topico)
        {
            topico.Ativo = true;
            topico.Dono = LoginUtils.Instance.Usuario;

            DbFactory.Instance.TopicoRepository.SaveOrUpdate(topico);

            return RedirectToAction("Topicos");
        }

        public ActionResult InativarTopico(Guid id)
        {
            var topico = DbFactory.Instance.TopicoRepository.FindFirstById(id);
            if (topico != null)
            {
                topico.Ativo = false;
                DbFactory.Instance.TopicoRepository.SaveOrUpdate(topico);
            }
            
            return RedirectToAction("Topicos");
        }

        public ActionResult Discussoes()
        {
            var discussoes = DbFactory.Instance.DiscussaoRepository.BuscarPelaSituacao(true);

            return View(discussoes);
        }

        public ActionResult AddDiscussao()
        {
            var discussao = new Discussao()
            {
                Dono = LoginUtils.Instance.Usuario,
                Ativo = true
            };

            var topicos = DbFactory.Instance.TopicoRepository.BuscarPelaSituacao(true);
            var selTopicos = new SelectList(topicos, "Id", "Titulo");
            ViewBag.Topicos = selTopicos;

            return View(discussao);
        }

        public ActionResult EditarDiscussao(Guid id)
        {
            var discussao = DbFactory.Instance.DiscussaoRepository.FindFirstById(id);
            if (discussao != null)
            {
                var topicos = DbFactory.Instance.TopicoRepository.BuscarPelaSituacao(true);
                var selTopicos = new SelectList(topicos, "Id", "Titulo", discussao.Topico.Id);
                ViewBag.Topicos = selTopicos;

                return View("AddDiscussao", discussao);
            }

            return RedirectToAction("Discussoes");
        }

        public ActionResult GravarDiscussao(Discussao discussao, Guid idTopico)
        {
            var topico = DbFactory.Instance.TopicoRepository.FindFirstById(idTopico);
            if (topico != null)
            {
                discussao.Ativo = true;
                discussao.Dono = LoginUtils.Instance.Usuario;
                discussao.Topico = topico;
                discussao.Criacao = DateTime.Now;

                DbFactory.Instance.DiscussaoRepository.SaveOrUpdate(discussao);
            }

            return RedirectToAction("Discussoes");
        }

        public ActionResult DeletarDiscussao(Guid id)
        {
            var discussao = DbFactory.Instance.DiscussaoRepository.FindFirstById(id);
            if (discussao != null)
            {
                discussao.Ativo = false;
                DbFactory.Instance.DiscussaoRepository.SaveOrUpdate(discussao);
            }

            return RedirectToAction("Discussoes");
        }

        

        public ActionResult Logar(String usuario, String senha)
        {
            LoginUtils.Instance.Logar(usuario, senha);

            return RedirectToAction("Discussoes");
        }

        public ActionResult Deslogar()
        {
            LoginUtils.Instance.Deslogar();

            return RedirectToAction("Index");
        }

        public ActionResult Perfil()
        {
            return View(LoginUtils.Instance.Usuario);
        }

        public ActionResult Comentarios(Guid id)
        {
            var comentarios = DbFactory.Instance.ComentarioRepository.BuscarPeloIdDiscussao(id);
            ViewBag.IdDiscussao = id;
            return View(comentarios);
        }

        public ActionResult AddComentario(Guid idDiscussao)
        {
            var discussao = DbFactory.Instance.DiscussaoRepository.FindFirstById(idDiscussao);
            if (discussao != null)
            {
                var comentario = new Comentario()
                {
                    Discussao = discussao,
                    Dono = LoginUtils.Instance.Usuario
                };

                return View(comentario);
            }

            return RedirectToAction("Comentarios", new { id = idDiscussao });
        }

        public ActionResult EditarComentario(Guid id, Guid idDiscussao)
        {
            var comentario = DbFactory.Instance.ComentarioRepository.FindFirstById(id);
            if (comentario != null)
            {
                return View("AddComentario", comentario);
            }

            return RedirectToAction("Comentarios", new { id = idDiscussao });
        }

        public ActionResult DeletarComentario(Guid id, Guid idDiscussao)
        {
            var comentario = DbFactory.Instance.ComentarioRepository.FindFirstById(id);
            if (comentario != null)
            {
                DbFactory.Instance.ComentarioRepository.Delete(comentario);
            }

            return RedirectToAction("Comentarios", new { id = idDiscussao });
        }
        
        public PartialViewResult GravarComentario(Comentario comentario, Guid idDiscussao)
        {
            var discussao = DbFactory.Instance.DiscussaoRepository.FindFirstById(idDiscussao);
            if (discussao != null)
            {
                comentario.Dono = LoginUtils.Instance.Usuario;
                comentario.Discussao = discussao;
                comentario.Criacao = DateTime.Now;
                
                DbFactory.Instance.ComentarioRepository.Save(comentario);
            }

            var comentarios = DbFactory.Instance.ComentarioRepository.BuscarPeloIdDiscussao(idDiscussao);
            ViewData["idDiscussao"] = idDiscussao;
            return PartialView("_TblComentarios", comentarios);
        }
    }
}