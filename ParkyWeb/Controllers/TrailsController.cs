using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ParkyWeb.Models;
using ParkyWeb.Models.ViewModel;
using ParkyWeb.Repository.IRepository;

namespace ParkyWeb.Controllers
{
    public class TrailsController : Controller
    {
        private readonly INationalParkRepository _npRepo;
        private readonly ITrailRepository _trailRepo;

        public TrailsController(INationalParkRepository npRepo , ITrailRepository trailRepo)
        {
            _npRepo = npRepo;
            _trailRepo = trailRepo;
        }

        public IActionResult Index()
        {
            return View(new Trail() { });
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            IEnumerable<NationalPark> npList = await _npRepo.GetAllAsync(SD.NationalParkAPIPath);

            TrailsVM objVM = new TrailsVM()
            {
                NationalParkList = npList.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                Trail = new Trail()
            };

            if (id == null)
            {
                //this will be true for Insert/Create
                return View(objVM);
            }

            //Flow will come here for update
            objVM.Trail = await _trailRepo.GetAsync(SD.TrailAPIPath, id.GetValueOrDefault());
            if (objVM.Trail == null)
            {
                return NotFound();
            }
            return View(objVM);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(TrailsVM opj)
        {
            if (ModelState.IsValid)
            {
                

                if(opj.Trail.Id == 0)
                {
                    await _trailRepo.CreateAsync(SD.TrailAPIPath, opj.Trail);
                }
                else
                {
                    await _trailRepo.UpdateAsync(SD.TrailAPIPath+opj.Trail.Id, opj.Trail);
                }

                return RedirectToAction(nameof(Index));
            }
            else
            {
                IEnumerable<NationalPark> npList = await _npRepo.GetAllAsync(SD.NationalParkAPIPath);

                TrailsVM objVM = new TrailsVM()
                {
                    NationalParkList = npList.Select(i => new SelectListItem
                    {
                        Text = i.Name,
                        Value = i.Id.ToString()
                    }),
                    Trail = new Trail()
                };


                return View(opj);
            }
         
        }

        public async Task<IActionResult> GetAllTrails()
        {
            return Json(new { data = await _trailRepo.GetAllAsync(SD.TrailAPIPath) });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var status = await _trailRepo.DeleteAsync(SD.TrailAPIPath , id);

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
