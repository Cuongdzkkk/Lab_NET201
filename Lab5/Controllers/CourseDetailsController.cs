using Lab5.Models;
using Lab5.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lab5.Controllers
{
    public class CourseDetailsController : Controller
    {
        private readonly ICourseDetailsService _courseDetailsService;
        private readonly ICourseService _courseService;
        private readonly ILogger<CourseDetailsController> _logger;

        public CourseDetailsController(
            ICourseDetailsService courseDetailsService,
            ICourseService courseService,
            ILogger<CourseDetailsController> logger)
        {
            _courseDetailsService = courseDetailsService;
            _courseService = courseService;
            _logger = logger;
        }

        // GET: CourseDetails
        public async Task<IActionResult> Index()
        {
            var courseDetails = await _courseDetailsService.GetAllCourseDetailsAsync();
            return View(courseDetails);
        }

        // GET: CourseDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseDetails = await _courseDetailsService.GetCourseDetailsByIdAsync(id.Value);
            if (courseDetails == null)
            {
                return NotFound();
            }

            return View(courseDetails);
        }

        // GET: CourseDetails/Create
        public async Task<IActionResult> Create()
        {
            var courses = await _courseService.GetAllCoursesAsync();
            ViewBag.CourseId = new SelectList(courses, "Id", "Name");
            return View();
        }

        // POST: CourseDetails/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseDetails courseDetails)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                ViewBag.Errors = errors;
                
                var courses = await _courseService.GetAllCoursesAsync();
                ViewBag.CourseId = new SelectList(courses, "Id", "Name", courseDetails.CourseId);
                return View(courseDetails);
            }

            try
            {
                await _courseDetailsService.CreateCourseDetailsAsync(courseDetails);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Errors = new List<string> { ex.InnerException?.Message ?? ex.Message };
                
                var courses = await _courseService.GetAllCoursesAsync();
                ViewBag.CourseId = new SelectList(courses, "Id", "Name", courseDetails.CourseId);
                return View(courseDetails);
            }
        }

        // GET: CourseDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseDetails = await _courseDetailsService.GetCourseDetailsByIdAsync(id.Value);
            if (courseDetails == null)
            {
                return NotFound();
            }
            
            var courses = await _courseService.GetAllCoursesAsync();
            ViewBag.CourseId = new SelectList(courses, "Id", "Name", courseDetails.CourseId);
            return View(courseDetails);
        }

        // POST: CourseDetails/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CourseDetails courseDetails)
        {
            if (id != courseDetails.CourseDetailsId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                ViewBag.Errors = errors;
                
                var courses = await _courseService.GetAllCoursesAsync();
                ViewBag.CourseId = new SelectList(courses, "Id", "Name", courseDetails.CourseId);
                return View(courseDetails);
            }

            try
            {
                await _courseDetailsService.UpdateCourseDetailsAsync(courseDetails);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                if (!await _courseDetailsService.CourseDetailsExistsAsync(id))
                {
                    return NotFound();
                }

                ViewBag.Errors = new List<string> { ex.InnerException?.Message ?? ex.Message };
                
                var courses = await _courseService.GetAllCoursesAsync();
                ViewBag.CourseId = new SelectList(courses, "Id", "Name", courseDetails.CourseId);
                return View(courseDetails);
            }
        }

        // GET: CourseDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseDetails = await _courseDetailsService.GetCourseDetailsByIdAsync(id.Value);
            if (courseDetails == null)
            {
                return NotFound();
            }

            return View(courseDetails);
        }

        // POST: CourseDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _courseDetailsService.DeleteCourseDetailsAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting course details {Id}", id);
                TempData["Error"] = "Error deleting course details: " + (ex.InnerException?.Message ?? ex.Message);
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
