using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace interfaz.console
{
  public class Pedidos : ICloneable
  {
    public string reference
    {
      get;
      set;
    }
    public string clasepedido
    {
      get { return ConfigurationManager.AppSettings["ClasePedido"]; }
    }
    public string ov
    {
      get { return ConfigurationManager.AppSettings["OrdenVenta"]; }
    }
    public string cdistribucion
    {
      get;
      set;
    }
    public string sector
    {
      get;
      set;
    }
    public DateTime fecha
    {
      get;
      set;
    }
    public string codigo
    {
      get;
      set;
    }
    public string condicionPago
    {
      get;
      set;
    }
    public string timpuesto
    {
      get;
      set;
    }
    public string tidentificacion
    {
      get;
      set;
    }
    public string nombre
    {
      get;
      set;
    }
    public string address1
    {
      get;
      set;
    }
    public string pais
    {
      get;
      set;
    }
    public string city
    {
      get;
      set;
    }
    public string moneda
    {
      get;
      set;
    }
    public string identificacion
    {
      get;
      set;
    }
    public string ruta
    {
      get;
      set;
    }
    public string ClienteCPD
    {
      get
      {
        return ConfigurationManager.AppSettings["clientepago"];
      }
    }
    public int posicion
    {
      get;
      set;
    }
    public string material
    {
      get;
      set;
    }
    public int cantidad
    {
      get;
      set;
    }
    public string centro
    {
      get { return ConfigurationManager.AppSettings["Centro"]; }
      
    }
    public double unitario
    {
      get;
      set;
    }
    public double descuento
    {
      get;
      set;
    }
    public string texto
    {
      get;
      set;
    }
    public double flete
    {
      get;
      set;
    }
    public Pedidos()
    {
    }
    public object Clone()
    {
      return this.MemberwiseClone();
    }
  }
}