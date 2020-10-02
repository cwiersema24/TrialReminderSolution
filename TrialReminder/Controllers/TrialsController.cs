using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrialReminder.Models;
using TrialReminder.Models.Trials;

namespace TrialReminder.Controllers
{
    public class TrialsController:Controller
    {
        private readonly TrialsDataContext _context;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _config;

        public TrialsController(TrialsDataContext context, IMapper mapper, MapperConfiguration config)
        {
            _context = context;
            _mapper = mapper;
            _config = config;
        }

        public async Task<IActionResult> IndexAsync()
        {
            var response = new TrialsSummaryModel();
            response.Trials = await _context.Trials.ProjectTo<TrialSummaryItemModel>(_config).ToListAsync();
            response.NumberOfCurrentTrials = response.Trials.Count();
            response.NumberOfExpiredTrials = response.Trials.Count(t => t.IsExpired);
            return View(response);
            
        }

        public ActionResult New()
        {
            return View(new TrialCreateModel());
        }
        [HttpPost]
        public async Task<ActionResult> CreateAsync(TrialCreateModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("New", model);
            }
            else
            {
                var trial = _mapper.Map<Trial>(model);
                _context.Trials.Add(trial);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            
        }
    }
}
