using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PARCIAL_completado
{
    public partial class frmTipoClienteEdit : Form
    {
        string cadenaconexion = @"server=localhost;DataBase=BancoBD;Integrated Security=true";
        int ID;
        public frmTipoClienteEdit(int id = 0)
        {
            InitializeComponent();
            this.ID = id;
        }

        private void AceptarCambios(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtnombre.Text))
            {
                MessageBox.Show("Debe ingresar un nombre valido","sistemas",
                    MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            this.DialogResult = DialogResult.OK;
        }

        private void CargarDatos(object sender, EventArgs e)
        {
            if (this.ID > 0)
            {
                this.Text = "Editar";
                
                mostrarDatos();
            }
        }


        void mostrarDatos()
        {
            using(var conexion=new SqlConnection(cadenaconexion))
            {
                conexion.Open();
                using (var comando=new SqlCommand("select*from TipoCliente where ID=@id",conexion))
                {
                    comando.Parameters.AddWithValue("@id",this.ID);
                    using (var reader = comando.ExecuteReader())
                    {
                        if(reader != null && reader.HasRows)
                        {
                            reader.Read();
                            txtnombre.Text = reader[1].ToString();
                            txtdescripcion.Text = reader[2].ToString();
                            chkestado.Checked = reader[3].ToString() == "1" ? true : false;
                        }
                    }
                }
            }
        }

        private void Cancelar(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
