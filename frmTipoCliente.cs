using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.Common;

namespace PARCIAL_completado
{
    public partial class frmTipoCliente : Form
    {
        string cadenaconexion = @"server=localhost;DataBase=BancoBD;Integrated Security=true";
        public frmTipoCliente()
        {
            InitializeComponent();
        }

        private void cargarFormulario(object sender, EventArgs e)
        {
            cargarDatos();
        }


        //private void cargarDatos()
        //{
        //    //var conexion = new SqlConnection(cadenaconexion);
        //    //conexion.Open();
        //    //var querysql = "select * from TipoCliente";
        //    //var comando = new SqlCommand(querysql,conexion);
        //    //var reader=comando.ExecuteReader();
        //    //if(reader!=null && reader.HasRows)
        //    //{
        //    //    reader.Read();
        //    //    dgvDatos.Rows.Add(reader[0], reader[1]) ;
        //    //}
        //    //conexion.Close();
        //}

        private void cargarDatos()
        {
            using(var conexion=new SqlConnection(cadenaconexion))//using:delimita el ciclo de vida de una variable
            {
                conexion.Open();
                using (var comando = new SqlCommand("select * from TipoCliente",conexion))
                {
                    using(var lector = comando.ExecuteReader())
                    {
                        if(lector!=null && lector.HasRows)
                        {
                            while (lector.Read())//mientras se pueda leer datos se agregaran al datagriedview 
                            {
                                dgvDatos.Rows.Add(lector[0], lector[1], lector[2], lector[3]);
                            }
                            
                            
                        }
                    }
                }
            }
        }

        private void NuevoRegistro(object sender, EventArgs e)
        {
            frmTipoClienteEdit frm = new frmTipoClienteEdit();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                string nombre = frm.Controls["txtnombre"].Text;
                string descripcion = frm.Controls["txtdescripcion"].Text;
                //operador ternario
                var estado = ((CheckBox)frm.Controls["chkestado"]).Checked == true ? 1:0;

                using(var conexion=new SqlConnection(cadenaconexion))
                {
                    conexion.Open();
                    using(var comando=new SqlCommand("insert into TipoCliente(Nombre, Descripcion, Estado)" +
                        "values (@nombre, @descripcion, @estado)", conexion))
                    {
                        comando.Parameters.AddWithValue("@nombre",nombre);
                        comando.Parameters.AddWithValue("@descripcion", descripcion);
                        comando.Parameters.AddWithValue("@estado", estado);
                        int resultado = comando.ExecuteNonQuery();
                        if (resultado > 0)
                        {
                            MessageBox.Show("Datos Registrados","sistemas",MessageBoxButtons.OK,MessageBoxIcon.Information);
                            
                        }
                        else
                        {
                            MessageBox.Show("no se ha podido registrar los datos", "sistemas", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        dgvDatos.Rows.Clear();
                        cargarDatos();
                    }
                }
            }
        }

        private void EditarRegistro(object sender, EventArgs e)
        {
            // VALIDAMOS QUE EXISTAN FILAS PARA EDITAR
            if (dgvDatos.RowCount > 0 && dgvDatos.CurrentRow != null)
            {
                // TOMAMOS EL ID DE LA FILA SELECCIONADA
                int idTipo = int.Parse(dgvDatos.CurrentRow.Cells[0].Value.ToString());
                var frm = new frmTipoClienteEdit(idTipo);
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    string nombre = ((TextBox)frm.Controls["txtNombre"]).Text;
                    string descripcion = ((TextBox)frm.Controls["txtDescripcion"]).Text;
                    // OPERADOR TERNARIO
                    var estado = ((CheckBox)frm.Controls["chkEstado"]).Checked == true ? 1 : 0;

                    using (var conexion = new SqlConnection(cadenaconexion))
                    {
                        conexion.Open();
                        using (var comando = new SqlCommand("UPDATE TipoCliente SET Nombre = @nombre, " +
                            "Descripcion = @descripcion, Estado = @estado WHERE ID = @id", conexion))
                        {
                            comando.Parameters.AddWithValue("@nombre", nombre);
                            comando.Parameters.AddWithValue("@descripcion", descripcion);
                            comando.Parameters.AddWithValue("@estado", estado);
                            comando.Parameters.AddWithValue("@id", idTipo);
                            int resultado = comando.ExecuteNonQuery();
                            if (resultado > 0)
                            {
                                MessageBox.Show("Datos actualizados.", "Sistemas",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("No se ha podido actualizar los datos.", "Sistemas",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            dgvDatos.Rows.Clear();
                            cargarDatos();
                        }
                    }
                }
            }
        }

        private void EliminarRegistro(object sender, EventArgs e)
        {
            string sql = "DELETE FROM TipoCliente WHERE ID = @rowID";
            if (dgvDatos.SelectedRows.Count == 0)
            {
                MessageBox.Show("no hay filas para eliminar","aviso",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
                return;  }
                using (SqlConnection conexion = new SqlConnection(cadenaconexion))
                {
                    using (SqlCommand comando = new SqlCommand(sql, conexion))
                    {
                        conexion.Open();
                        int indiceseleccionado = dgvDatos.SelectedRows[0].Index;
                        int rowID = Convert.ToInt32(dgvDatos[0,indiceseleccionado].Value);
                        comando.Parameters.Add("@rowID", SqlDbType.Int).Value = rowID;
                        comando.ExecuteNonQuery();
                        dgvDatos.Rows.RemoveAt(indiceseleccionado);
                    }
                
                }
        }

        private void Salir(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
