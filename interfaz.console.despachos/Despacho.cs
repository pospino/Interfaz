using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace interfaz.console.despachos
{
  public class Despacho
  {
    public string id_cart { get; set; }
    public string Guia { get; set; }
    public string Fecha_Envio { get; set; }
    public string Transportador { get; set; }
    public string Fecha_Recibido { get; set; }
    public string COmentario_Transportador { get; set; }
    public string No_Entrega { get; set; }
    public string Pedido_SAP { get; set; }
    public string Factura_SAP { get; set; }
    public Estados Estado { get; set; }

  }
  public enum Estados
  {
    Preparacion = 3,
    Enviado = 4,
    Entregado = 5
  }
}
