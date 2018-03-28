using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Student6.Data;
using Student6.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Xrm.Sdk;

namespace Student6.Controllers
{
    [Authorize]
    public class InquiriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _UserManager;

        public InquiriesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _UserManager = userManager;
            _context = context;    
        }

        // GET: Inquiries
        public async Task<IActionResult> Index()
        {
            var inquiries = _context.Inquiry.Where(i => i.UserId == _UserManager.GetUserId(User));
            return View(inquiries);
        }

        // GET: Inquiries/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inquiry = await _context.Inquiry
                .SingleOrDefaultAsync(m => m.InquiryId == id);

            var service = CRM.CrmService.GetServiceProvider();
            Entity crmInquiry = service.Retrieve("drew_inquiry", inquiry.InquiryId, new Microsoft.Xrm.Sdk.Query.ColumnSet("drew_response"));
            inquiry.Response = crmInquiry.GetAttributeValue<string>("drew_response");

            if (inquiry == null)
            {
                return NotFound();
            }

            return View(inquiry);
        }

        // GET: Inquiries/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Inquiries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("InquiryId,Question,Response,UserId")] Inquiry inquiry)
        {
            if (ModelState.IsValid)
            {
                inquiry.InquiryId = Guid.NewGuid();
                inquiry.UserId = _UserManager.GetUserId(User);
                _context.Add(inquiry);
                await _context.SaveChangesAsync();

                Entity crmInquiry = new Entity("drew_inquiry");
                crmInquiry.Id = inquiry.InquiryId;
                crmInquiry["drew_question"] = inquiry.Question;
                crmInquiry["drew_response"] = inquiry.Response;
                crmInquiry["drew_contact"] = new EntityReference("contact", Guid.Parse(_UserManager.GetUserId(User)));

                var service = CRM.CrmService.GetServiceProvider();
                service.Create(crmInquiry);

                return RedirectToAction("Index");
            }
            return View(inquiry);
        }

        // GET: Inquiries/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inquiry = await _context.Inquiry.SingleOrDefaultAsync(m => m.InquiryId == id);
            if (inquiry == null)
            {
                return NotFound();
            }
            return View(inquiry);
        }

        // POST: Inquiries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("InquiryId,Question,Response,UserId")] Inquiry inquiry)
        {
            if (id != inquiry.InquiryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    inquiry.UserId = _UserManager.GetUserId(User);
                    _context.Update(inquiry);
                    await _context.SaveChangesAsync();

                    var service = CRM.CrmService.GetServiceProvider();
                    Entity CrmInquiry = service.Retrieve("drew_inquiry", inquiry.InquiryId, new Microsoft.Xrm.Sdk.Query.ColumnSet("drew_question"));

                    CrmInquiry["drew_question"] = inquiry.Question;
                    service.Update(CrmInquiry);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InquiryExists(inquiry.InquiryId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(inquiry);
        }

        // GET: Inquiries/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inquiry = await _context.Inquiry
                .SingleOrDefaultAsync(m => m.InquiryId == id);

            if (inquiry == null)
            {
                return NotFound();
            }

            return View(inquiry);
        }

        // POST: Inquiries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var inquiry = await _context.Inquiry.SingleOrDefaultAsync(m => m.InquiryId == id);
            _context.Inquiry.Remove(inquiry);

            var service = CRM.CrmService.GetServiceProvider();
            service.Delete("drew_inquiry", inquiry.InquiryId);

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool InquiryExists(Guid id)
        {
            return _context.Inquiry.Any(e => e.InquiryId == id);
        }
    }
}
