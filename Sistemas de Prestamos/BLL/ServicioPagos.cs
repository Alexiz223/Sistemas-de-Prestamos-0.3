using System;
using System.Data;
using Sistemas_de_Prestamos.DAL;

namespace Sistemas_de_Prestamos.BLL
{
    public class PagoService
    {
        private PagosDAL pagosDAL = new PagosDAL();
        private PrestamoService prestamoService = new PrestamoService();

        // Registrar pago tomando el NombreCliente desde Prestamos
        public int RegistrarPago(int prestamoID, decimal montoPagado, DateTime fechaPago, string estado, decimal mora = 0)
        {
            // Buscar el préstamo para obtener el NombreCliente
            DataRow prestamo = prestamoService.ObtenerPrestamo(prestamoID);

            if (prestamo == null)
                throw new Exception("Préstamo no encontrado.");

            string nombreCliente = prestamo["NombreCliente"].ToString();

            // Guardar el pago con el nombre del cliente
            return pagosDAL.RegistrarPago(prestamoID, nombreCliente, montoPagado, fechaPago, estado, mora);
        }

        // Consultar todos los pagos
        public DataTable ConsultarPagos()
        {
            return pagosDAL.ConsultarPagos();
        }

        // Obtener pago específico
        public DataRow ObtenerPago(int pagoID)
        {
            return pagosDAL.ObtenerPago(pagoID);
        }

        // Editar pago
        public void EditarPago(int pagoID, decimal montoPagado, DateTime fechaPago, string estado, decimal mora = 0)
        {
            pagosDAL.EditarPago(pagoID, montoPagado, fechaPago, estado, mora);
        }

        // Eliminar pago
        public void EliminarPago(int pagoID)
        {
            pagosDAL.EliminarPago(pagoID);
        }

        // Obtener nombre del cliente desde PrestamoID (para mostrar en el formulario)
        public string ObtenerNombreClienteDesdePrestamo(int prestamoID)
        {
            DataRow prestamo = prestamoService.ObtenerPrestamo(prestamoID);

            if (prestamo == null)
                throw new Exception("Préstamo no encontrado.");

            return prestamo["NombreCliente"].ToString();
        }
    }
}