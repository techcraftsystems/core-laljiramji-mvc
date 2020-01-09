using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Core.Extensions;
using Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Core.Services
{
    public class ProductService : IProductService
    {
        private CoreService CoreService = new CoreService();

        public List<SelectListItem> GetProductsIEnumerable(Stations station) {
            string group = "SELECT DISTINCT Category FROM " + station.Prefix + "Products WHERE Category NOT LIKE '%N/A%';";
            string query = "SELECT id_, Items, Category FROM " + station.Prefix + "pProducts WHERE id_>10 AND Category NOT LIKE '%N/A%' ORDER BY Category, Items";
            return CoreService.GetIEnumerableGroup(group, query);
        }

        public List<ProductsTransactions> GetProductsTransactions(Stations station, Products product, DateTime from, DateTime ends) {
            List<ProductsTransactions> items = new List<ProductsTransactions>();

            SqlServerConnection conn = new SqlServerConnection();
            SqlDataReader dr = conn.SqlServerConnect("USE " + station.Prefix.Replace(".dbo.","") + "; DECLARE @item INT=" + product.Id + ", @from DATE='" + from.Date + "', @ends DATE='" + ends.Date + "'; SELECT item_idnt, Items, Category, dt, descr, transc,[IN],[OUT] FROM (SELECT 1 line, id_ item_idnt, Items, Category, @from dt, 'OPENING BALANCE' descr, 88 transc, uAvailable-SUM(ISNULL(CASE WHEN pm_direction=1 THEN qnty ELSE 0-qnty END,0)) [IN],0[OUT] FROM pProducts LEFT OUTER JOIN ProductsMovement ON id_=item_idnt AND CAST(date_ AS DATE)>=@from LEFT OUTER JOIN ProductsMovementSources ON srcs_idnt=pm_idnt WHERE id_=@item GROUP BY id_, uAvailable, Items, Category UNION ALL SELECT 2, item_idnt, Items, Category, CAST(date_ AS DATE)dt, pm_movement, tsid_idnt, CASE WHEN pm_direction=1 THEN qnty ELSE 0 END [IN], CASE WHEN pm_direction=0 THEN qnty ELSE 0 END [OUT] FROM ProductsMovement INNER JOIN ProductsMovementSources ON srcs_idnt=pm_idnt INNER JOIN pProducts ON id_=item_idnt WHERE item_idnt=@item AND CAST(date_ AS DATE) BETWEEN @from AND @ends) As Foo ORDER BY line, dt");
            if (dr.HasRows) {
                while (dr.Read()) {
                    items.Add(new ProductsTransactions {
                        Product = new Products {
                            Id = Convert.ToInt64(dr[0]),
                            Name = dr[1].ToString(),
                            Category = dr[2].ToString()
                        },
                        Date = Convert.ToDateTime(dr[3]).ToString("dd/MM/yyyy"),
                        CreatedOn = Convert.ToDateTime(dr[3]),
                        Description = dr[4].ToString(),
                        Reference = dr[5].ToString(),
                        In = Convert.ToDouble(dr[6]),
                        Out = Convert.ToDouble(dr[7])
                    });
                }
            }


            return items;
        }

    }
}
