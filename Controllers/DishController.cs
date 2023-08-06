using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.Models.Dish;
using RestaurantAPI.Services;

namespace RestaurantAPI.Controllers
{
    [Route("api/restaurant/{restaurantId}/dish")]
    [ApiController]
    public class DishController : ControllerBase
    {
        private readonly IDishService _dishService;

        public DishController(IDishService dishService)
        {
            this._dishService = dishService;
        }

        [HttpPost]
        public ActionResult Create([FromRoute] int restaurantId, [FromBody]CreateDishDto createDishDto)
        {
            var newDishId = _dishService.Create(restaurantId, createDishDto);

            return Created($"api/restaurant/{restaurantId}/dish/{newDishId}", null);
        }

        [HttpDelete("{dishId}")]
        public ActionResult Delete([FromRoute] int restaurantId, [FromRoute] int dishId)
        {
            _dishService.Delete(restaurantId, dishId);

            return NoContent();
        }

        [HttpDelete]
        public ActionResult DeleteAll([FromRoute] int restaurantId)
        {
            _dishService.DeleteAll(restaurantId);

            return NoContent();
        }

        [HttpGet("{dishId}")]
        public ActionResult<DishDto> Get([FromRoute] int restaurantId, [FromRoute] int dishId)
        {
            var dishDto = _dishService.GetById(restaurantId, dishId);

            return Ok(dishDto);
        }

        [HttpGet]
        public ActionResult<IEnumerable<DishDto>> GetAll([FromRoute] int restaurantId, [FromRoute] int dishId)
        {
            var dishDtos = _dishService.GetAll(restaurantId);

            return Ok(dishDtos);
        }
    }
}
