using System.Threading.Tasks;
using Crpg.Application.Common.Results;
using Crpg.Application.Games.Commands;
using Crpg.Application.Games.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Crpg.WebApi.Controllers
{
    [Authorize(Policy = GamePolicy)]
    public class GamesController : BaseController
    {
        /// <summary>
        /// All-in-One endpoint to get or create users with character, give gold and experience, and break/repair items.
        /// </summary>
        [HttpPut("update")]
        public Task<ActionResult<Result<UpdateGameResult>>> Update([FromBody] UpdateGameCommand cmd) =>
            ResultToActionAsync(Mediator.Send(cmd));
    }
}
