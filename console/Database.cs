using FluentData;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace interfaz.console
{
  public class Database : IDisposable
	{
		private static IDbContext context;
    public static IDbContext Context(string _Conexion)
    {
      
        Database.context = (new DbContext()).ConnectionStringName(_Conexion, new MySqlProvider());

        return Database.context;
      
    }

		static Database()
		{
			Database.context = null;
		}

		public Database()
		{
      
		}

		public void Dispose()
		{
			if (Database.context != null)
			{
				Database.context.Dispose();
			}
		}
    public List<Pagos> getPagos()
    {
      List<Pagos> pagos = getPagos("MySql").Union(getPagos("VC_Mysql")).ToList();
      return pagos;
    }

    public List<Pagos> getPagos(string _Conexion)
		{
      DateTime now ;
      now = DateTime.Now.AddDays(double.Parse(ConfigurationManager.AppSettings["DiasPagos"]));
			string str = " SELECT DATE( op.date_add ) fdoc, DATE( NOW( ) ) fcon, id_cart AS oc,  "+
        "  DATE( op.date_add ) fvalor, o.total_paid_tax_incl neto,  "+
        " concat(firstname,' ',lastname) nombre, dni cedula  "+
        " FROM ps_orders o   INNER JOIN ps_address a ON a.id_customer = o.id_customer "+
        " AND o.id_address_delivery = a.id_address "+
        " INNER JOIN ps_order_payment op ON o.reference = op.order_reference "+
        "  WHERE current_state  IN ( 2, 3, 4, 5 ) "+
        " AND date(o.date_add) = date(@dia) ";
			List<Pagos> pagos = Database.Context(_Conexion).Sql(str, new object[0])
                .Parameter("dia", now,parameterType: DataTypes.Date)
                .QueryMany<Pagos>(null);
      return pagos;
		}

    public List<Pedidos> getPedidos()
    {
      List<Pedidos> pedidos = getPedidos("MySql");
      List<Pedidos> pedidosVC = getPedidos("VC_MySql");
      return pedidos.Union(pedidosVC).ToList();
    }
		public List<Pedidos> getPedidos(string _Conexion)
		{
			string str;
			Pedidos pedido;
			string[] strArrays;
			int num;
      DateTime now = DateTime.Now.AddDays(double.Parse(ConfigurationManager.AppSettings["DiasPedidos"]));
      string str1 = 
                " SELECT o.id_cart 'reference',     'EC' cdistribucion,  'PD' sector,  "+
                " 'PN'timpuesto,   'CC'tidentificacion, CONCAT( a.firstname,  ' ', a.lastname ) nombre, a.address1, 'CO' pais, "+
                " a.city,  'COP'moneda,  a.dni identificacion,'' codigo,  0 posicion, p.reference material,   "+
                " 'ruta' ruta,  ROUND(p.price) unitario, od.product_quantity cantidad, od.reduction_percent descuento,  "+
                " DATE( o.date_add ) fecha, o.total_shipping_tax_excl flete  "+
                " FROM ps_orders o  INNER JOIN ps_address a ON o.id_address_invoice = a.id_address "+
                " INNER JOIN ps_country_lang cl ON a.id_country = cl.id_country AND cl.id_lang =1  "+
                " INNER JOIN ps_order_detail od ON o.id_order = od.id_order "+
                " INNER JOIN ps_product p ON p.id_product = od.product_id "+
                " WHERE o.current_state IN ( 12, 2,4 )   "+
                " AND date(o.date_add) = date(@dias) ";
      List<Pedidos> pedidos = Database.Context(_Conexion).Sql(str1, new object[0])
                .Parameter("dias", now, parameterType: DataTypes.Date)
                .QueryMany<Pedidos>(null);
			List<Pedidos> pedidos1 = new List<Pedidos>();
            string sql_referencia = " SELECT price FROM  `ps_product` WHERE reference =  @referencia ";
			foreach (Pedidos pedido1 in pedidos)
			{
				int num1 = 1;
				string str2 = pedido1.city;
				char[] chrArray = new char[] { '-' };
				string[] strArrays1 = str2.Split(chrArray);
				pedido1.ruta = strArrays1[0];
				pedido1.city = ((int)strArrays1.Length > 1 ? strArrays1[1] : strArrays1[0]);
				string str3 = pedido1.material;
				chrArray = new char[] { '/' };
				string[] strArrays2 = str3.Split(chrArray);
				int num2 = strArrays2.Count<string>();
				if (num2 <= 1)
				{
					str = pedido1.material;
					pedido = (Pedidos)pedido1.Clone();
					chrArray = new char[] { 'X' };
					strArrays = str.Split(chrArray);
                    if (strArrays.Count<string>() <= 1)
					{
						int num3 = num1;
						num1 = num3 + 1;
						pedido.posicion = num3;
						pedido.material = str.Trim();

					}
					else
					{
						num = int.Parse(strArrays[1]);
						num = num * pedido.cantidad;
						pedido.unitario = pedido.unitario / (double)num;
						pedido.material = strArrays[0].Trim();
						pedido.cantidad = num;
						int num4 = num1;
						num1 = num4 + 1;
						pedido.posicion = num4;
					}
                    pedido.unitario = Database.Context(_Conexion).Sql(sql_referencia, new object[0])
                        .Parameter("referencia", pedido.material)
                        .QuerySingle<double>();
					pedidos1.Add(pedido);
				}
				else
				{
					string[] strArrays3 = strArrays2;
                    for (int i = 0; i < (int)strArrays3.Length; i++)
                    {
                        str = strArrays3[i];
                        pedido = (Pedidos)pedido1.Clone();
                        pedido.unitario = pedido1.unitario / (double)num2;
                        chrArray = new char[] { 'X' };
                        strArrays = str.Split(chrArray);
                        if (strArrays.Count<string>() <= 1)
                        {
                            int num5 = num1;
                            num1 = num5 + 1;
                            pedido.posicion = num5;
                            pedido.material = str.Trim();
                        }
                        else
                        {
                            num = int.Parse(strArrays[1]);
                            num = num * pedido.cantidad;
                            pedido.unitario = pedido.unitario / (double)num;
                            pedido.material = strArrays[0].Trim();
                            pedido.cantidad = num;
                            int num6 = num1;
                            num1 = num6 + 1;
                            pedido.posicion = num6;
                        }
                        pedido.unitario = Database.Context(_Conexion).Sql(sql_referencia, new object[0])
                            .Parameter("referencia", pedido.material)
                            .QuerySingle<double>();
                        pedidos1.Add(pedido);
                    }
				}
				if (pedido1.flete != 0)
				{
					pedido = (Pedidos)pedido1.Clone();
					pedido.cantidad = 1;
					pedido.descuento = 0;
					pedido.material = ConfigurationManager.AppSettings["FleteIVA"];
					int num7 = num1;
					num1 = num7 + 1;
					pedido.posicion = num7;
					pedido.unitario = pedido.flete;
					pedidos1.Add(pedido);
				}
				
			}
      var distinct = pedidos1.DistinctBy(x => x.reference);
      foreach(var d in distinct)
      {
        int pos = 1;
        pedidos1.Where(x => x.reference == d.reference).ForEach(y =>
          {
            y.posicion = pos;
            pos++;
          });
      }
			return pedidos1;
		}
	}
}