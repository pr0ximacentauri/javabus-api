using Npgsql;
using System.Data;

namespace javabus_api.Helpers
{
    public class SqlDBhelper
    {
        private NpgsqlConnection connection;
        private string __constr;
        public SqlDBhelper(string pConstr)
        {
            __constr = pConstr;
            connection = new NpgsqlConnection(__constr);
        }

        public NpgsqlCommand GetNpgsqlCommand(string query)
        {
            connection.Open();
            NpgsqlCommand cmd = new NpgsqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = query;
            cmd.CommandType = CommandType.Text;
            return cmd;
        }

        public void closeConnection()
        {
            connection.Close();
        }
    }
}
