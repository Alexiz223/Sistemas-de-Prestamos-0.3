using Servicios_de_Amortizacion;
using Servicios_de_Amortizacion.BLL;
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

namespace Sistemas_de_Prestamos.Forms
{
    public partial class FrmAmortizacion : Form
    {
        public FrmAmortizacion()
        {
            InitializeComponent();
        }
        public static class SesionFormularios
        {
            public static FrmPrestamos frmPrestamos = new FrmPrestamos();
            public static Clientes frmClientes = new Clientes();
            public static FrmPagos frmPagos = new FrmPagos();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Guardar lo que está escrito en los TextBox de Amortización
            SesionAmortizacion.NombreCliente = txtbox_Nombre.Text;

            if (decimal.TryParse(txtbox_monto_deseado.Text, out decimal monto))
                SesionAmortizacion.MontoDeseado = monto;

            if (decimal.TryParse(txtbox_Sueldo.Text, out decimal sueldo))
                SesionAmortizacion.Sueldo = sueldo;

            if (int.TryParse(DTP_1.Text, out int plazo))
                SesionAmortizacion.PlazoMeses = plazo;

            if (int.TryParse(txtBox_Moras.Text, out int moras))
                SesionAmortizacion.Moras = moras;

            SesionAmortizacion.FechaInicio = DTP_2.Value;

            // Ahora sí abrir Clientes
            SesionFormularios.frmClientes.Show();
            this.Hide();

        }

        public static class SesionAmortizacion
        {
            public static string NombreCliente { get; set; }
            public static decimal MontoDeseado { get; set; }
            public static decimal Sueldo { get; set; }
            public static int PlazoMeses { get; set; }
            public static int Moras { get; set; }
            public static DateTime FechaInicio { get; set; }
        }


        private void button1_Click(object sender, EventArgs e)
        {

        }
            private void btnCalcular_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtbox_monto_deseado.Text) || string.IsNullOrEmpty(txtbox_Sueldo.Text))
                {
                    MessageBox.Show("Por favor, llene el monto y el sueldo.");
                    return;
                }

                // Limpiar símbolos y convertir a decimal
                string montoLimpio = txtbox_monto_deseado.Text.Replace(",", "").Replace("$", "").Trim();
                string sueldoLimpio = txtbox_Sueldo.Text.Replace(",", "").Replace("$", "").Trim();

                decimal MontoDeseado = decimal.Parse(montoLimpio, System.Globalization.CultureInfo.InvariantCulture);
                decimal sueldo = decimal.Parse(sueldoLimpio, System.Globalization.CultureInfo.InvariantCulture);

                // Instancia del BLL
                AmortizacionBLL guardar = new AmortizacionBLL();

                decimal FondoActual = guardar.ConsultarFondoBanco();
                int moras = int.Parse(txtBox_Moras.Text);

                int TiempoEntrada = int.Parse(DTP_1.Text);
                int MesesFinales = (DTP_1.Text == "Años") ? TiempoEntrada * 12 : TiempoEntrada;

                DateTime FechaInicio = DTP_2.Value;

                string ResultadoValidacion = guardar.ValidarReglas(sueldo, MontoDeseado, FondoActual, moras);

                if (ResultadoValidacion == "OK")
                {
                    txtBox_Tasa_Anual.Enabled = false;

                    // Tasa anual fija (puedes ajustar si la calculas desde BD)
                    decimal tea = 0.18m;
                    txtBox_Tasa_Anual.Text = tea.ToString();
                    DTP_2.Text = MesesFinales.ToString();

                    double tem = guardar.CalcularTEM((double)tea);
                    txt_TEM.Text = (tem * 100).ToString("N2") + "%";

                    var ListaCuotas = guardar.GenerarCuotas(MontoDeseado, (double)tea, MesesFinales, FechaInicio);
                    dgv_Cuotas.DataSource = null;
                    dgv_Cuotas.DataSource = ListaCuotas;

                    if (dgv_Cuotas.Columns["FechaVencimiento"] != null)
                    {
                        dgv_Cuotas.Columns["FechaVencimiento"].DefaultCellStyle.Format = "dd/MM/yyyy";
                        dgv_Cuotas.Columns["FechaVencimiento"].HeaderText = "F. Vencimiento";
                    }

                    dgv_Cuotas.Columns["MontoCuota"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    txtbox_cuotas.Text = ListaCuotas.Sum(c => c.MontoCuota).ToString("N2");

                    Guardar.Enabled = true;
                }
                else
                {
                    MessageBox.Show(ResultadoValidacion, "Cliente no apto", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dgv_Cuotas.DataSource = null;
                    Guardar.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Verifique que los campos numéricos estén correctos. Detalles: " + ex.Message,
                        "Error de Formato", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
      

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void txtbox_Nombre_TextChanged(object sender, EventArgs e)
        {
            txtbox_Nombre.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtbox_Nombre.AutoCompleteSource = AutoCompleteSource.CustomSource;
        }

        private void CargarClientesEnAutocompletado()
        {
            AutoCompleteStringCollection nombresClientes = new AutoCompleteStringCollection();

            using (SqlConnection conn = new SqlConnection("Server=.\\SQLEXPRESS;Database=SistemaPrestamosLogin;Trusted_Connection=True;"))
            {
                conn.Open();
                string query = "SELECT NombreCliente FROM Clientes"; // Ajusta si tu columna se llama diferente

                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    nombresClientes.Add(reader["NombreCliente"].ToString());
                }
            }

            txtbox_Nombre.AutoCompleteCustomSource = nombresClientes;
        }

        private void FrmAmortizacion_Load(object sender, EventArgs e)
        {
            txtbox_Nombre.Text = SesionAmortizacion.NombreCliente;
            txtbox_monto_deseado.Text = SesionAmortizacion.MontoDeseado != 0 ? SesionAmortizacion.MontoDeseado.ToString("N2") : "";
            txtbox_Sueldo.Text = SesionAmortizacion.Sueldo != 0 ? SesionAmortizacion.Sueldo.ToString("N2") : "";
            DTP_1.Text = SesionAmortizacion.PlazoMeses != 0 ? SesionAmortizacion.PlazoMeses.ToString() : "";
            txtBox_Moras.Text = SesionAmortizacion.Moras.ToString();
            if (SesionAmortizacion.FechaInicio != DateTime.MinValue)
                DTP_2.Value = SesionAmortizacion.FechaInicio;

        }

        private void FrmAmortizacion_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                // Guardar lo que el usuario escribió en sesión
                SesionAmortizacion.NombreCliente = txtbox_Nombre.Text;

                if (decimal.TryParse(txtbox_monto_deseado.Text, out decimal monto))
                    SesionAmortizacion.MontoDeseado = monto;

                if (decimal.TryParse(txtbox_Sueldo.Text, out decimal sueldo))
                    SesionAmortizacion.Sueldo = sueldo;

                if (int.TryParse(DTP_1.Text, out int plazo))
                    SesionAmortizacion.PlazoMeses = plazo;

                if (int.TryParse(txtBox_Moras.Text, out int moras))
                    SesionAmortizacion.Moras = moras;

                SesionAmortizacion.FechaInicio = DTP_2.Value;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar datos de amortización: " + ex.Message);
            }
        }


    }
}

