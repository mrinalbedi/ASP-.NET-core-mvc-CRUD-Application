using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MBPatients.Models;

namespace MBPatients.Controllers
{
    public class MBPatientController : Controller
    {
        private readonly PatientsContext _context;

        public MBPatientController(PatientsContext context)
        {
            _context = context;
        }

        // GET: MBPatient
        public async Task<IActionResult> Index()
        {
            var patientsContext = _context.Patient.Include(p => p.ProvinceCodeNavigation).OrderBy(m=>m.LastName).ThenBy(m=>m.FirstName);
            return View(await patientsContext.ToListAsync());
        }

        // GET: MBPatient/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patient
                .Include(p => p.ProvinceCodeNavigation)
                .FirstOrDefaultAsync(m => m.PatientId == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // GET: MBPatient/Create
        public IActionResult Create()
        {
            ViewData["ProvinceCode"] = new SelectList(_context.Province, "ProvinceCode", "ProvinceCode");
            return View();
        }

        // POST: MBPatient/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PatientId,FirstName,LastName,Address,City,ProvinceCode,PostalCode,Ohip,DateOfBirth,Deceased,DateOfDeath,HomePhone,Gender")] Patient patient)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    patient.ProvinceCode = patient.ProvinceCode.Substring(0, 2);
                    _context.Add(patient);
                    await _context.SaveChangesAsync();
                    TempData["message"] = $"Record successfully added for patient: {patient.LastName+','+patient.FirstName}";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.GetBaseException().Message);
            }
            
            ViewData["ProvinceCode"] = new SelectList(_context.Province, "ProvinceCode", "ProvinceCode", patient.ProvinceCode);
            return View(patient);
        }

        // GET: MBPatient/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patient.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }
            ViewData["ProvinceCode"] = new SelectList(_context.Province.OrderBy(m=>m.Name), "ProvinceCode", "Name");
            return View(patient);
        }

        // POST: MBPatient/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PatientId,FirstName,LastName,Address,City,ProvinceCode,PostalCode,Ohip,DateOfBirth,Deceased,DateOfDeath,HomePhone,Gender")] Patient patient)
        {
            if (id != patient.PatientId)
            {
                ModelState.AddModelError("", "you're not updating the record you requested");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patient);
                    await _context.SaveChangesAsync();
                    TempData["message"] = $"Record updated successfully for patient : {patient.LastName+','+patient.FirstName}";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientExists(patient.PatientId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch(Exception e)
                {
                    ModelState.AddModelError("", e.GetBaseException().Message);
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProvinceCode"] = new SelectList(_context.Province.OrderBy(m => m.Name), "ProvinceCode", "Name", patient.ProvinceCode);
            return View(patient);
        }

        // GET: MBPatient/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patient
                .Include(p => p.ProvinceCodeNavigation)
                .FirstOrDefaultAsync(m => m.PatientId == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // POST: MBPatient/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var patient = await _context.Patient.FindAsync(id);
                _context.Patient.Remove(patient);
                await _context.SaveChangesAsync();
                TempData["message"] = $"Patient record for: '{patient.LastName+','+patient.FirstName}' deleted from database";
                return RedirectToAction(nameof(Index));

            }
            catch (Exception e)
            {
                TempData["message"] = "Error in deleting the record from the database:" + e.GetBaseException().Message;
            }
            return RedirectToAction("Delete");
            
        }

        private bool PatientExists(int id)
        {
            return _context.Patient.Any(e => e.PatientId == id);
        }
    }
}
