using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Models;

namespace Queo.Boards.Controllers {
    /// <summary>
    ///     Testcontroller
    /// </summary>
    [RoutePrefix("api")]
    public class NSwagTestController : ApiController {

        /// <summary>
        ///     Liefert einen Testwert
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("test")]
        public async Task<IHttpActionResult> GetTestValue() {
            return Ok("42");
        }

        [HttpGet]
        [Route("test/authenticate")]
        [Queo.Boards.Infrastructure.Http.Authorize]
        public IHttpActionResult TestAuthentication() {
            return Ok("Mensch, Gratulation, du bist identifiziert!");
        }

        [HttpGet]
        [Route("test/authorize")]
        [Queo.Boards.Infrastructure.Http.Authorize()]
        public IHttpActionResult TestAuthorize() {
            return Ok("Mensch, Gratulation, du bist autorisiert!");
        }

        

        //public IHttpActionResult TestBinding([ModelBinder]Board board, TestBindingCommand testBindingCommand) {
        //[Route("{board}")]

        //[HttpPost]

        //    return Ok(new TestBindingModel() {
        //        Cards = CardModelBuilder.Build(testBindingCommand.Card),
        //        Cards = testBindingCommand.Cards.Select(CardModelBuilder.Build).ToList(),
        //        Board = BoardModelBuilder.Build(board),
        //        List = ListModelBuilder.Build(testBindingCommand.TestBindingDto.List),
        //        ListId = testBindingCommand.TestBindingDto.ListId,
        //        SinnDesLebens =  testBindingCommand.TestBindingDto.SinnDesLebens
        //    });
        //}
    }

    public class TestBindingModel {
        public BoardModel Board { get; set; }

        public CardModel CardModel { get; set; }

        public IList<CardModel> Cards { get; set; }

        public ListModel List { get; set; }

        public Guid ListId { get; set; }

        public int SinnDesLebens { get; set; }
    }

    public class TestBindingCommand {
        public Card Card { get; set; }

        public IList<Card> Cards { get; set; }

        public TestBindingDto TestBindingDto { get; set; }
    }

    public class TestBindingDto {
        public List List { get; set; }

        public Guid ListId { get; set; }

        public int SinnDesLebens { get; set; }
    }
}