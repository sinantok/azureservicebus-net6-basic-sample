using CoreLibrary;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Helper;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContentController : ControllerBase
    {
        private readonly AzureServiceHelper _azureServiceHelper;
        public ContentController(AzureServiceHelper azureServiceHelper)
        {
            _azureServiceHelper = azureServiceHelper;
        }

        [HttpPost]
        public async Task<IActionResult> CreateContent(int id)
        {
            var contentCreatedEvent = new ContentCreatedEvent()
            {
                Id = id,
                CreateTime = DateTime.Now,
                ContentType = "Article"
            };

            await _azureServiceHelper.CreateTopicIfNotExists(Constants.ContentTopic);
            await _azureServiceHelper.CreateSubscriptionIfNotExists(Constants.ContentTopic, Constants.ContentCreatedSubName, "ContentCreated", "ContentCreatedOnly");

            await _azureServiceHelper.SendMessageToTopic(Constants.ContentTopic, contentCreatedEvent, "ContentCreated");

            //await azureService.CreateQueueIfNotExists(Constants.ContentCreatedQueueName);
            //await azureService.SendMessageToQueue(Constants.ContentCreatedQueueName, contentCreatedEvent);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletecontent(int id)
        {
            var contentDeletedEvent = new ContentDeletedEvent()
            {
                Id = id,
                CreateTime = DateTime.Now
            };

            await _azureServiceHelper.CreateTopicIfNotExists(Constants.ContentTopic);
            await _azureServiceHelper.CreateSubscriptionIfNotExists(Constants.ContentTopic, Constants.ContentDeletedSubName, "ContentDeleted", "ContentDeletedOnly");

            await _azureServiceHelper.SendMessageToTopic(Constants.ContentTopic, contentDeletedEvent, "ContentDeleted");

            //await azureService.CreateQueueIfNotExists(Constants.ContentDeletedQueueName);
            //await azureService.SendMessageToQueue(Constants.ContentDeletedQueueName, ContentDeletedEvent);

            return Ok();
        }
    }
}
