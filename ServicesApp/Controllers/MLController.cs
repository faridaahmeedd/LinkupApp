﻿//using Microsoft.AspNetCore.Mvc;
//using ServicesApp.Interfaces;
//using ServicesApp.Models;
//using ServicesApp.Repositories;

//namespace ServicesApp.Controllers
//{
//    [Route("/api/[controller]")]
//    [ApiController]
//    public class MLController : ControllerBase
//    {
//        private readonly IMLRepository _ml;

//        public MLController(IMLRepository ml)
//        {
//            _ml = ml;
//        }

//        [HttpPost]
//        public async Task<IActionResult> MatchJobAndService(int serviceId , string jobtitle)
//        {
//            bool result = await _ml.MatchJobAndService(serviceId , jobtitle);
//            return Ok(result);
//        }
//    }
//}
