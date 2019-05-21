using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ForumVianna.Model.DB.Model;
using ForumVianna.Model.DB.Repository;
using MySql.Data.MySqlClient;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Context;
using NHibernate.Mapping.ByCode;

namespace ForumVianna.Model.DB
{
    public class DbFactory
    {
        private static DbFactory _instance = null;

        private ISessionFactory _sessionFactory;

        public UsuarioRepository UsuarioRepository { get; set; }
        public TopicoRepository TopicoRepository { get; set; }
        public DiscussaoRepository DiscussaoRepository { get; set; }
        public ComentarioRepository ComentarioRepository { get; set; }

        private DbFactory()
        {
            Conectar();

            this.UsuarioRepository = new UsuarioRepository(this.Session);
            this.TopicoRepository = new TopicoRepository(this.Session);
            this.DiscussaoRepository = new DiscussaoRepository(this.Session);
            this.ComentarioRepository = new ComentarioRepository(this.Session);
        }

        public static DbFactory Instance => _instance ?? (_instance = new DbFactory());

        public void Initialize(object obj)
        {
            NHibernateUtil.Initialize(obj);
        }

        private void Conectar()
        {
            try
            {
                var server = "localhost";
                var port = "3306";
                var dbName = "vianna_forum";
                var user = "root";
                var psw = "!@#$1234";

                var stringConexao = "Persist Security Info=False;server=" + server + ";port=" + port + ";database=" +
                                    dbName + ";uid=" + user + ";pwd=" + psw;

                var mySql = new MySqlConnection(stringConexao);
                try
                {
                    mySql.Open();
                }
                catch
                {
                    CriarSchemaBanco(server, port, dbName, psw, user);
                }
                finally
                {
                    if (mySql.State == ConnectionState.Open)
                    {
                        mySql.Close();
                    }
                }

                ConexaoNHibernate(stringConexao);
            }
            catch (Exception ex)
            {
                throw new Exception("Não foi possível conectar ao banco de dados.", ex);
            }
        }

        private void CriarSchemaBanco(string server, string port, string dbName, string psw, string user)
        {
            try
            {
                var stringConexao = "server=" + server + ";user=" + user + ";port=" + port + ";password=" + psw + ";";
                var mySql = new MySqlConnection(stringConexao);
                var cmd = mySql.CreateCommand();

                mySql.Open();
                cmd.CommandText = "CREATE DATABASE IF NOT EXISTS `" + dbName + "`;";
                cmd.ExecuteNonQuery();
                mySql.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("Não foi criar o banco de dados.", ex);
            }
        }

        private void ConexaoNHibernate(string stringConexao)
        {
            //Cria a configuração com o NH
            var config = new Configuration();
            try
            {
                //Exibição de SQL no console
                ConfigureLog4Net();

                //Integração com o Banco de Dados
                config.DataBaseIntegration(c =>
                {
                    //Dialeto de Banco
                    c.Dialect<NHibernate.Dialect.MySQLDialect>();
                    //Conexao string
                    c.ConnectionString = stringConexao;
                    //Drive de conexão com o banco
                    c.Driver<NHibernate.Driver.MySqlDataDriver>();
                    // Provedor de conexão do MySQL 
                    c.ConnectionProvider<NHibernate.Connection.DriverConnectionProvider>();
                    // GERA LOG DOS SQL EXECUTADOS NO CONSOLE
                    c.LogSqlInConsole = true;
                    // DESCOMENTAR CASO QUEIRA VISUALIZAR O LOG DE SQL FORMATADO NO CONSOLE
                    c.LogFormattedSql = true;
                    // CRIA O SCHEMA DO BANCO DE DADOS SEMPRE QUE A CONFIGURATION FOR UTILIZADA
                    c.SchemaAction = SchemaAutoAction.Update;
                });

                //Realiza o mapeamento das classes
                var maps = this.Mapeamento();
                config.AddMapping(maps);

                //Verifico se a aplicação é Desktop ou Web
                if (HttpContext.Current == null)
                {
                    config.CurrentSessionContext<ThreadStaticSessionContext>();
                }
                else
                {
                    config.CurrentSessionContext<WebSessionContext>();
                }

                this._sessionFactory = config.BuildSessionFactory();
            }
            catch (Exception ex)
            {
                throw new Exception("Não foi possível conectar o NHibernate.", ex);
            }
        }

        private HbmMapping Mapeamento()
        {
            try
            {
                var mapper = new ModelMapper();

                mapper.AddMappings(
                    Assembly.GetAssembly(typeof(UsuarioMap)).GetTypes()
                );

                return mapper.CompileMappingForAllExplicitlyAddedEntities();
            }
            catch (Exception ex)
            {
                throw new Exception("Não foi possível realizar o mapeamento do modelo.", ex);
            }
        }

        public ISession Session
        {
            get
            {
                try
                {
                    if (CurrentSessionContext.HasBind(_sessionFactory))
                        return _sessionFactory.GetCurrentSession();

                    var session = _sessionFactory.OpenSession();
                    session.FlushMode = FlushMode.Commit;

                    CurrentSessionContext.Bind(session);

                    return session;
                }
                catch (Exception ex)
                {
                    throw new Exception("Não foi possível criar a Sessão.", ex);
                }
            }
        }

        public void ClearSession()
        {
            this.Session.Clear();
        }

        public static void ConfigureLog4Net()
        {
            log4net.Config.XmlConfigurator.Configure();

            /***
            Exemplo de configuração do log para nhibernate no arquivo app.config
            -------------------------------------------------------------------
              <configSections>
                <section name="log4net"
                  type="log4net.Config.Log4NetConfigurationSectionHandler, log4net"/>
              </configSections>
              <log4net>
                <appender name="NHLog" type="log4net.Appender.RollingFileAppender, log4net" >
                  <param name="File" value="NHLog.txt" />
                  <param name="AppendToFile" value="true" />
                  <param name="maximumFileSize" value="200KB" />
                  <param name="maxSizeRollBackups" value="1" />
                  <layout type="log4net.Layout.PatternLayout, log4net">
                    <conversionPattern
                    value="%date{yyyy.MM.dd hh:mm:ss} %-5level [%thread] - %message%newline" />
                  </layout>
                </appender>
                <!-- levels: ALL, DEBUG, INFO, WARN, ERROR, FATAL, OFF -->
                <root>
                  <level value="INFO" />
                  <appender-ref ref="NHLog" />
                </root>
                <logger name="NHBase.SQL">
                  <level value="ALL" />
                  <appender-ref ref="NHLog" />
                </logger>
              </log4net>
            ***/
        }
    }
}
