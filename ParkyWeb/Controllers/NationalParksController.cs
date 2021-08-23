using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ParkyWeb.Models;
using ParkyWeb.Repository.IRepository;

namespace ParkyWeb.Controllers
{
    public class NationalParksController : Controller
    {
        private readonly INationalParkRepository _npRepo;

        public NationalParksController(INationalParkRepository npRepo)
        {
            _npRepo = npRepo;
        }

        public IActionResult Index()
        {
            return View(new NationalPark() { });
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            NationalPark opj = new NationalPark();

            if(id == null)
            {
                // thats is for create

                return View(opj);

            }
            else
            {
                //thats for Update
                opj = await _npRepo.GetAsync(SD.NationalParkAPIPath, id.GetValueOrDefault());
                if(opj == null)
                {
                    return NotFound();
                }
                return View(opj);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(NationalPark opj)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if(files.Count > 0)
                {
                    //thats mean there is picture uploaded
                    byte[] p1 = null;
                    using (var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }
                    }

                    opj.Picture = p1;
                }
                else
                {
                    var objFromDb = await _npRepo.GetAsync(SD.NationalParkAPIPath, opj.Id);
                    opj.Picture = objFromDb.Picture;
                }

                if(opj.Id == 0)
                {
                    await _npRepo.CreateAsync(SD.NationalParkAPIPath, opj);
                }
                else
                {
                    await _npRepo.UpdateAsync(SD.NationalParkAPIPath+opj.Id, opj);
                }

                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(opj);
            }
         
        }

        public async Task<IActionResult> GetAllNationalParks()
        {
            return Json(new { data = await _npRepo.GetAllAsync(SD.NationalParkAPIPath) });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var status = await _npRepo.DeleteAsync(SD.NationalParkAPIPath , id);

            if (status)
            {
                return Json(new { success = true, message = "Delete Successful" });
            }
            else {
                return Json(new { success = false, message = "Delete Not Successful" });

            }
        }
    }
}
