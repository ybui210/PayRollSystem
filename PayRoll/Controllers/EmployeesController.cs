﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PayRoll.Models;

namespace PayRoll.Controllers
{
    public class EmployeesController : Controller
    {
        private PayrollDbContext db = new PayrollDbContext();

        // GET: Employees
        public ActionResult Index()
        {
            return View(db.Employees.ToList());
        }

        // GET: Employees/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // GET: Employees/Create
        public ActionResult Create()
        {
			ViewData["positions"] = db.Positions.ToArray();
			return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Password,FName,LName,Address,Email,FullOrPartTime,Seniority,DepartmentType,HourlyRate")] Employee employee)
        {
			employee.EmployeeId = generateEmployeeId();
            if (ModelState.IsValid)
            {
                db.Employees.Add(employee);
                db.SaveChanges();
				db.Positions.Find(Request.Form.Get("Position")).Employees.Add(employee);
				db.SaveChanges();
				return RedirectToAction("Index");
            }

            return View(employee);
        }

        // GET: Employees/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EmployeeId,Password,FName,LName,Address,Email,FullOrPartTime,Seniority,DepartmentType")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(employee);
        }

        // GET: Employees/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
			try
			{
				Employee employee = db.Employees.Find(id);
				db.Attendances.RemoveRange(db.Attendances.Where(e => e.Employee.EmployeeId == id));
				db.TimeOffRequests.RemoveRange(db.TimeOffRequests.Where(e => e.Employee.EmployeeId == id));
				db.Payrolls.RemoveRange(db.Payrolls.Where(e => e.Employee.EmployeeId == id));
				db.Entry(employee).State = EntityState.Modified;
				db.SaveChanges();
				db.Employees.Remove(employee);
				db.SaveChanges();
			} catch (Exception ex)
			{
			}
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

		private string generateEmployeeId()
		{
			Random rnd = new Random();
			string result = "a00";
			for (int i = 0; i < 6; i++)
			{
				result += rnd.Next(0, 10);
			}
			return result;
		}
        
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(String Email, String password)
        {
            //Email Refers to EmployeeId textbox in Login page.
                var myEmployee = db.Employees
                      .FirstOrDefault(u => u.EmployeeId == Email
                                   && u.Password == password);

            if (myEmployee != null)
            {
                Session["EmployeeId"] = db.Employees.FirstOrDefault().EmployeeId;
                return RedirectToAction("Index", "PayrollManage");
            }
            else
            {
                ModelState.AddModelError("", "Invalid login credentials.");
                return RedirectToAction("Login","Account");
            }
       
        }
        }

    }


