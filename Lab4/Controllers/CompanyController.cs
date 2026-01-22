using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Lab4.Models;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.VisualBasic;
using Microsoft.EntityFrameworkCore;
using Lab4.Migrations;
using Lab4.Data;

namespace Lab4.Controllers;

public class CompanyController : Controller
{
    private readonly CompanyDbContext _context;

    public CompanyController(CompanyDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult CreateInformation()
    {
        return View();
    }

    [HttpPost]
    public IActionResult CreateInformation(Company info)
    {
        if (ModelState.IsValid)
        {
            _context.Companies.Add(info);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        return View(info);
    }
}