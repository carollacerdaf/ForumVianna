using ForumVianna.Model.DB;
using ForumVianna.Model.DB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ForumVianna.Site.Utils
{
    public class LoginUtils
    {
        public Usuario Usuario { get
            {
                if(HttpContext.Current.Session["Usuario"] != null)
                {
                    return (Usuario)HttpContext.Current.Session["Usuario"];
                }

                return null;
            }
        }
 
        private static LoginUtils _instance = null;

        private LoginUtils()
        {
            
        }

        public static LoginUtils Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new LoginUtils();

                return _instance;
            }
        }

        public Usuario Logar(String login, String senha)
        {
            var usuario = DbFactory.Instance.UsuarioRepository.obterUsuario(login, senha);

            if(usuario != null)
            {
                HttpContext.Current.Session["Usuario"] = usuario;
                return usuario;
            }

            return null;
        }

        public void Deslogar()
        {
            HttpContext.Current.Session["Usuario"] = null;
            HttpContext.Current.Session.Remove("Usuario");            
        }
    }
}