using System;
namespace Core.Models
{
    public class ReportVatBreakdown
    {
        public DateTime date { get; set; }

        public double dx_sales { get; set; }
        public double ux_sales { get; set; }
        public double vp_sales { get; set; }
        public double ik_sales { get; set; }
        public double tt_sales { get; set; }

        public double dx_credit { get; set; }
        public double ux_credit { get; set; }
        public double vp_credit { get; set; }
        public double ik_credit { get; set; }
        public double tt_credit { get; set; }

        public double dx_discs { get; set; }
        public double ux_discs { get; set; }
        public double vp_discs { get; set; }
        public double ik_discs { get; set; }
        public double tt_discs { get; set; }

        public double dx_lesses { get; set; }
        public double dx_transp { get; set; }
        public double dx_cashes { get; set; }
        
        public double dx_price { get; set; }
        public double ux_price { get; set; }
        public double vp_price { get; set; }
        public double ik_price { get; set; }
        
        public double dx_zero { get; set; }
        public double ux_zero { get; set; }
        public double vp_zero { get; set; }
        public double ik_zero { get; set; }

        public double dx_ltrs { get; set; }
        public double ux_ltrs { get; set; }
        public double vp_ltrs { get; set; }
        public double ik_ltrs { get; set; }

        public double vt_vatab { get; set; }
        public double vt_vatsx { get; set; }
        public double vt_zeros { get; set; }
        public double vt_total { get; set; }

        public double cr_vatab { get; set; }
        public double cr_vatsx { get; set; }
        public double cr_zeros { get; set; }
        public double cr_total { get; set; }

        public double dxc_ltrs { get; set; }
        public double uxc_ltrs { get; set; }
        public double vpc_ltrs { get; set; }
        public double ikc_ltrs { get; set; }

        public double ca_vatab { get; set; }
        public double ca_vatsx { get; set; }
        public double ca_zeros { get; set; }
        public double ca_total { get; set; }

        public ReportVatBreakdown() {
            date = DateTime.Now;

            dx_sales = 0;
            ux_sales = 0;
            vp_sales = 0;
            ik_sales = 0;
            tt_sales = 0;

            dx_credit = 0;
            ux_credit = 0;
            vp_credit = 0;
            ik_credit = 0;
            tt_credit = 0;

            dx_discs = 0;
            ux_discs = 0;
            vp_discs = 0;
            ik_discs = 0;
            tt_discs = 0;

            dx_price = 0;
            ux_price = 0;
            vp_price = 0;
            ik_price = 0;

            dx_zero = 0;
            ux_zero = 0;
            vp_zero = 0;
            ik_zero = 0;

            dx_lesses = 0;
            dx_transp = 0;
            dx_cashes = 0;

            dx_ltrs = 0;
            ux_ltrs = 0;
            vp_ltrs = 0;
            ik_ltrs = 0;

            vt_vatab = 0;
            vt_vatsx = 0;
            vt_zeros = 0;
            vt_total = 0;

            cr_vatab = 0;
            cr_vatsx = 0;
            cr_zeros = 0;
            cr_total = 0;

            dxc_ltrs = 0;
            uxc_ltrs = 0;
            vpc_ltrs = 0;
            ikc_ltrs = 0;

            ca_vatab = 0;
            ca_vatsx = 0;
            ca_zeros = 0;
            ca_total = 0;
        }
    }
}
