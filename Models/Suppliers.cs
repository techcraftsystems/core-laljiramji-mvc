﻿using System;
using Core.Services;
using Microsoft.AspNetCore.Http;

namespace Core.Models
{
    public class Suppliers
    {
        public long Id { get; set; }
        public string Uuid { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Tabs { get; set; }
        public string Pin { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public double Balance { get; set; }
        public bool Fuel { get; set; }
        public bool Lube { get; set; }
        public bool Gas { get; set; }
        public bool Soda { get; set; }

        public Suppliers() {
            Id = 0;
            Uuid = "";
            Name = "";
            Icon = "";
            Tabs = "";
            Pin = "";
            Address = "";
            City = "";
            Telephone = "";
            Email = "";
        }

        public Suppliers(long idnt) : this() {
            Id = idnt;
        }

        public Suppliers(long idnt, string name) : this() {
            Id = idnt;
            Name = name;
        }

        public Suppliers Save() {
            return new CoreService().SaveSuppliers(this);
        }
    }

    public class SuppliersPayment
    {
        public long Id { get; set; }
        public long Type { get; set; }
        public DateTime Date { get; set; }
        public string DateString { get; set; }
        public string Receipt { get; set; }
        public string Cheque { get; set; }
        public string Invoices { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public Suppliers Supplier { get; set; }
        public Bank Bank { get; set; }
        public Users User { get; set; }

        public SuppliersPayment()
        {
            Id = 0;
            Type = 0;
            Date = DateTime.Now;
            DateString = Date.ToString("dd/MM/yyyy");
            Receipt = "";
            Cheque = "";
            Invoices = "";
            Description = "";
            Amount = 0;
            Supplier = new Suppliers();
            User = new Users();
        }

        public SuppliersPayment(long idnt) : this()
        {
            Id = idnt;
        }

        public SuppliersPayment Save(HttpContext Context)
        {
            return new CoreService(Context).SaveSuppliersPayment(this);
        }

        public void Delete()
        {
            new CoreService().DeleteSuppliersPayment(this);
        }
    }

    public class SuppliersCredits
    {
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public string DateString { get; set; }
        public SuppliersCreditsType Type { get; set; }
        public Suppliers Supplier { get; set; }
        public Stations Station { get; set; }
        public string Receipt { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public Users AddedBy { get; set; }
        public DateTime AddedOn { get; set; }

        public SuppliersCredits()
        {
            Id = 0;
            Date = DateTime.Now;
            DateString = Date.ToString("dd/MM/yyyy");
            Type = new SuppliersCreditsType();
            Supplier = new Suppliers();
            Station = new Stations();
            Receipt = "";
            Description = "";
            Amount = 0;
            AddedBy = new Users();
            AddedOn = DateTime.Now;
        }

        public SuppliersCredits(long idnt) : this()
        {
            Id = idnt;
        }

        public SuppliersCredits Save(HttpContext Context)
        {
            return new CoreService(Context).SaveCreditNote(this);
        }

        public void Delete()
        {
            new CoreService().DeleteSuppliersCredit(this);
        }
    }

    public class SuppliersCreditsType
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public SuppliersCreditsType()
        {
            Id = 0;
            Code = "";
            Name = "";
        }
    }

    public class SuppliersWithholding {
        public long Id { get; set; }
        public Suppliers Supplier { get; set; }
        public Bank Bank { get; set; }
        public Types Type { get; set; }
        public Users User { get; set; }
        public DateTime Date { get; set; }
        public string DateString { get; set; }
        public string Receipt { get; set; }
        public string Invoice { get; set; }
        public string Cheque { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }

        public SuppliersWithholding() {
            Id = 0;
            Date = DateTime.Now;
            DateString = Date.ToString("dd/MM/yyyy");
            Receipt = "";
            Invoice = "";
            Cheque = "";
            Description = "";
            Amount = 0;
            Supplier = new Suppliers();
            User = new Users();
            Type = new Types();
        }

        public SuppliersWithholding(long idnt) : this() {
            Id = idnt;
        }

        public SuppliersWithholding Save(HttpContext Context) {
            return new CoreService(Context).SaveSuppliersWithholding(this);
        }

        public void Delete() {
            new CoreService().DeleteSuppliersWithholding(this);
        }
    }

    public class SuppliersStatement {
        public long Id { get; set; }
        public Suppliers Supplier { get; set; }
        public Statement Statement { get; set; }

        public SuppliersStatement() {
            Id = 0;
            Supplier = new Suppliers();
            Statement = new Statement();
        }
    }
}
