using System;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
namespace EBA
{
    internal static class Connection
    {
        internal static MySqlConnection con; //todo: private
        internal static void Initialize()
        {
            try
            {
                con = new MySqlConnection("SERVER=" + Props.ip +";DATABASE=eba;UID=root;PASSWORD=howdoiturnthison;USECOMPRESSION=true");
                
                con.Open();
            }
            catch (Exception e)
            {
                MessageBox.Show("No se pudo conectar al servidor\n" + e.Message);
                Environment.Exit(1);
            }
            if (con.State == System.Data.ConnectionState.Open) //conecto ok
            {
                //todo: mover a queries
                if (!Queries.validVersion())
                {
                    if (MessageBox.Show("La version del programa no coincide con la de la base de datos.\nQueres bajar la nueva version?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start("http://" + Props.ip +"/archivos/EBA.exe");
                    }
                    Environment.Exit(1);
                }
                return;
            }

            MessageBox.Show(con.State.ToString());
        }

        internal static MySqlCommand CreateCommand()
        {
            return con.CreateCommand();
        }

    }
}
